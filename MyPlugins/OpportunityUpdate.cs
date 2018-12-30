using System;

using System.Collections.Generic;

using System.Linq;

using System.Text;

using System.Threading.Tasks;

using Microsoft.Xrm.Sdk;

using System.ServiceModel;

using Microsoft.Xrm.Sdk.Query;

namespace MyPlugins

{

    public class OpportunityUpdate : IPlugin

    {

        public void Execute(IServiceProvider serviceProvider)

        {

            ITracingService tracingService =

                (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.  

            IPluginExecutionContext context = (IPluginExecutionContext)

                serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Obtain the organization service reference which you will need for  

            // web service calls.  

            IOrganizationServiceFactory serviceFactory =

                (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            // The InputParameters collection contains all the data passed in the message request.  

            if (context.InputParameters.Contains("Target") &&

                context.InputParameters["Target"] is Entity)

            {

                // Obtain the target entity from the input parameters.  

                Entity opportunity = (Entity)context.InputParameters["Target"];

                try

                {

                    //Assumption: Run Plugin Only When Opportunity title is "Test", you replace this with any attribute and value

                    if (opportunity.Attributes.Contains("title") && opportunity.Attributes["title"].ToString() == "Test")

                    {

                        //I am assuming custom entity "new_branch" and lookup attribute name (on Branch Entity) is new_opportunityid

                        // Get all branches which are relate to the current opportunity

                        QueryExpression query = new QueryExpression("new_branch");

                        query.ColumnSet.AddColumns(new string[] { "new_opportunityid" });

                        query.Criteria.AddCondition("new_opportunityid",

                            ConditionOperator.Equal, opportunity.Id);

                        EntityCollection collection = service.RetrieveMultiple(query);

                        foreach (Entity branch in collection.Entities)

                        {

                            // Create a case

                            Entity incident = new Entity("incident");

                            incident.Attributes.Add("title", "Some case title");

                            // To create a case, either contact or account is mandatory. 

                            // I am assuming your customer is same as contact field present on opprtunity entity.

                            // I created another function "GetContactGUIDfromOpportuntiy" to retrieve contact from opportunity.

                            incident.Attributes.Add("customerid",

                                GetContactGUIDfromOpportuntiy(opportunity, context));

                            service.Create(incident);

                        }

                    }

                    else

                    {

                        //If Title is not Test then skip the Plugin execution

                        return;

                    }

                }

                catch (FaultException<OrganizationServiceFault> ex)

                {

                    throw new InvalidPluginExecutionException("An error occurred in MyPlug-in.", ex);

                }

                catch (Exception ex)

                {

                    tracingService.Trace("MyPlugin: {0}", ex.ToString());

                    throw;

                }

            }

        }

        internal EntityReference GetContactGUIDfromOpportuntiy(Entity opportunity, IPluginExecutionContext context)

        {

            EntityReference contactRef = null;

            if (opportunity.Attributes.Contains("parentcontactid"))

            {

                // User is updating contact also.

                contactRef = (EntityReference)opportunity.Attributes["parentcontactid"];

            }

            else

            {

                // User is NOT updating contact, so get it from image

                // You need to register pre-entity image with "PreImage" key

                Entity image = (Entity)context.PreEntityImages["PreImage"];

                if (image.Attributes.Contains("parentcontactid"))

                {

                    contactRef = (EntityReference)image.Attributes["parentcontactid"];

                }

            }

            return contactRef;

        }

    }

}

