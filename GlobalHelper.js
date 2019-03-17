var Helper = window.Helper || {};
(
    function () {

        this.DoSomething = function (executionContext) {

            alert("This is from Helper file");
        }

    

    }
).call(Helper);