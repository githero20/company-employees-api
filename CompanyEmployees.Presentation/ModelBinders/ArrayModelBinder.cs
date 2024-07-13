using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;
using System.Reflection;

namespace CompanyEmployees.Presentation.ModelBinders
{
    //This is because our API can’t bind the string type parameter to the
    //IEnumerable<Guid> argument in the GetCompanyCollection action.
    //Well, we can solve this with a custom model binding
    public class ArrayModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            // the model binder is of the IEnumerable type, so it has to match
            if (!bindingContext.ModelMetadata.IsEnumerableType)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            // provided value here is our comma separated string of GUIDs
            var providedValue = bindingContext.ValueProvider
                .GetValue(bindingContext.ModelName)
                .ToString();
            // if the value is null, we can return null because we have a null check in our action in the controller
            if(string.IsNullOrEmpty(providedValue) )
            {
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }

            // we store the type IEnumerable consists of
            var genericType =
                bindingContext.ModelType.GetTypeInfo().GenericTypeArguments[0];
            // create a converter for whatever type that has been saved
            var converter = TypeDescriptor.GetConverter(genericType);

            // create an array of type object that consists of all the GUID values
            // that were sent to the API
            var objectArray = providedValue.Split(new[] { "," },
               StringSplitOptions.RemoveEmptyEntries)
                .Select(x => converter.ConvertFromString(x.Trim()))
                .ToArray();

            // create an array of GUID types, 
            var guidArray = Array.CreateInstance(genericType, objectArray.Length);
            // copy all the values from the object array above
            objectArray.CopyTo(guidArray, 0);
            // assign to bindingContext
            bindingContext.Model = guidArray;

            bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);
            return Task.CompletedTask;
        }
    }
}
