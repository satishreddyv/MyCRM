
var Sdk = window.Sdk || {};
(
    function () {

        this.formOnLoad = function (executionContext) {

            Helper.DoSomething();




            var formContext = executionContext.getFormContext();

            var formtype = formContext.ui.getFormType();

            if (formtype == 1) {
                formContext.ui.setFormNotification("User is creating account", "INFO", "1");


            }
            else if (formtype == 2) {
                formContext.ui.setFormNotification("User is opening existing account", "INFO", "2");
            }
            else if (formtype == 3) {
                formContext.ui.setFormNotification("User doesnot have permissions to edit the record", "INFO", "3");
            }
        }

        this.formOnSave = function (executionContext) {
            var eventArgs = executionContext.getEventArgs();
            if (eventArgs.getSaveMode() == 70) {
                eventArgs.preventDefault();
            }
        }

        this.MainPhoneOnChange = function (executionContext) {
            var formContext = executionContext.getFormContext();
            var phoneNumber = formContext.getAttribute("telephone1").getValue();

            var expression = /((\(\d{3}\) ?)|(\d{3}-))?\d{3}-\d{4}/;

            if (!expression.test(phoneNumber)) {

                //Use field notification to display error message so that Users cannot save the form
                formContext.getControl("telephone1").setNotification("Enter phone number in US format",
                    "telephonemsg");

                formContext.ui.setFormNotification("Info message", "INFO", "formnoti1");

            }
            else {
                //Clear the notification
                formContext.getControl("telephone1").clearNotification("telephonemsg");
                formContext.ui.clearFormNotification("formnoti1");
            }

        }

    }
).call(Sdk);