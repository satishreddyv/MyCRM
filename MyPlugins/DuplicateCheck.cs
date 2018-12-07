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
    public class DuplicateCheck : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Extract the tracing service for use in debugging sandboxed plug-ins.  
            // If you are not registering the plug-in in the sandbox, then you do  
            // not have to add any tracing service related code.  
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
                Entity contact = (Entity)context.InputParameters["Target"];


                try
                {
                    // Plug-in business logic goes here.  
                    string email = string.Empty;

                    if (contact.Attributes.Contains("emailaddress1"))
                    {
                        email = contact.Attributes["emailaddress1"].ToString();

                        // select * from contact where emailaddress1 == 'email'

                        QueryExpression query = new QueryExpression("contact");
                        query.ColumnSet = new ColumnSet(new string[] { "emailaddress1" });
                        query.Criteria.AddCondition("emailaddress1", ConditionOperator.Equal, email);

                        EntityCollection collection = service.RetrieveMultiple(query);

                        if(collection.Entities.Count > 0)
                        {
                            throw new InvalidPluginExecutionException("Contact with email already exists!!!");
                        }
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
    }
}
