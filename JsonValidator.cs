using Cdw.Azure.Function.Subscription.Vendor.Connector.Model.Interfaces;
using Cdw.Azure.Function.Subscription.Vendor.Connector.Model.Validators;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Cdw.Azure.Function.Subscription.Vendor.Connector.Services.Validators
{
    public class JsonValidator : IJsonValidator
    {
        private JSchema _schema;
        private string _fileName;

        public JsonValidator(string fileName)
        {
            _fileName = fileName;
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), _fileName);
            var schemaString = File.ReadAllText(filePath);
            _schema = JSchema.Parse(schemaString);
        }

        public async Task<ValidationResult> ValidateJsonSchemaAsync(string jsonString)
        {
            return await ValidateJsonSchemaAsync(JObject.Parse(jsonString)).ConfigureAwait(false);
        }
    
        public async Task<ValidationResult> ValidateJsonSchemaAsync(JObject json)
        {
            return await Task.Run(() =>
            {
                var isValid = json.IsValid(_schema, out IList<ValidationError> validationErrors);

                return new ValidationResult
                {
                    IsValid = isValid,
                    JsonErrors = validationErrors,
                    ParsedJson = json,
                    SchemaFileName = _fileName
                };
            }).ConfigureAwait(false);
        }
    }
}