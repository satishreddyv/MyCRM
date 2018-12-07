using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using System.ServiceModel;

namespace MyPlugins
{
    public class AccountUpdate : IPlugin
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
                Entity account = (Entity)context.InputParameters["Target"];


                try
                {  
                    // Plug-in business logic goes here.  
                    decimal revenue = 0;
                    int numberofemployees = 0;
                    // IF the value is entered by the user while updating
                    if (account.Attributes.Contains("revenue") && account.Attributes["revenue"] != null)
                    {
                         revenue = ((Money)account.Attributes["revenue"]).Value;
                    }
                    else
                    {
                        // service.Retrieve("account",account.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet("")
                        // Using Pre-entity Image
                        Entity image = context.PreEntityImages["PreImage"];

                        // Checking for attribute because, attribute may not be availble in the collection
                        // if it is blank. 
                        if (image.Attributes.Contains("revenue"))
                        {
                            
                            revenue = ((Money)image.Attributes["revenue"]).Value;
                        }
                        else
                        {
                            // Skipping Plugin
                            return;
                        }
                    }

                    if (account.Attributes.Contains("numberofemployees") && account.Attributes["numberofemployees"] != null)
                    {
                        numberofemployees = Convert.ToInt32(account.Attributes["numberofemployees"].ToString());
                    }
                    else
                    {
                        // service.Retrieve("account",account.Id, new Microsoft.Xrm.Sdk.Query.ColumnSet("")
                        // Using Pre-entity Image
                        Entity image = context.PreEntityImages["PreImage"];

                        // Checking for attribute because, attribute may not be availble in the collection
                        // if it is blank. 
                        if (image.Attributes.Contains("numberofemployees"))
                        {

                            numberofemployees = Convert.ToInt32(image.Attributes["numberofemployees"].ToString());
                        }
                        else
                        {
                            // Skipping Plugin
                            return;
                        }
                    }

                    if(revenue == 0 || numberofemployees == 0 )
                    {
                        // Make it blank again
                        account.Attributes.Add("contoso_annualrevenueperemployee", null);
                        return;
                    }

                    decimal revenueperemployee = revenue / numberofemployees;

                    account.Attributes.Add("contoso_annualrevenueperemployee", new Money(revenueperemployee));

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
