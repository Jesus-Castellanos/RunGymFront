using System;
using System.Globalization;
using System.Web.Mvc;

namespace RunGymFront.Binders
{
    public class DecimalModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            var modelState = new ModelState { Value = valueResult };

            if (valueResult != null && !string.IsNullOrWhiteSpace(valueResult.AttemptedValue))
            {
                decimal actualValue;
                // Reemplaza coma por punto para culturas tipo "es-ES"
                string attempted = valueResult.AttemptedValue.Replace(",", ".");

                if (decimal.TryParse(attempted, NumberStyles.Any, CultureInfo.InvariantCulture, out actualValue))
                {
                    return actualValue;
                }

                modelState.Errors.Add("Valor decimal inválido");
                bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
            }

            return base.BindModel(controllerContext, bindingContext);
        }
    }
}