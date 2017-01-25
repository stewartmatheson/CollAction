using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace CollAction.MetaForm {

  class CustomResolver : DefaultContractResolver
  {
      protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
      {
          JsonProperty property = base.CreateProperty(member, memberSerialization);

          property.ShouldSerialize = instance =>
          {
              try
              {
                  PropertyInfo prop = (PropertyInfo)member;
                  if (prop.CanRead)
                  {
                      prop.GetValue(instance, null);
                      return true;
                  }
              }
              catch
              {
              }
              return false;
          };

          return property;
      }
  }

  class Validation {
    public Validation(Attribute a) {
      Type = a.ToString();
      if (a.ToString() == "System.ComponentModel.DataAnnotations.RequiredAttribute") {
        ErrorMessage = ((RequiredAttribute)a).ErrorMessage;
      }

      if (a.ToString() == "System.ComponentModel.DataAnnotations.DisplayAttribute") {
        DisplayName = ((DisplayAttribute)a).Name;
      }

      if (a.ToString() == "System.ComponentModel.DataAnnotations.MaxLengthAttribute") {
        MaxLength = ((MaxLengthAttribute)a).Length;
      }
    }

    public string Type { get; set; }
    public int MaxLength { get; set; }
    [JsonProperty( NullValueHandling = NullValueHandling.Ignore )]
    public string ErrorMessage { get; set; }
    [JsonProperty( NullValueHandling = NullValueHandling.Ignore )]
    public string DisplayName { get; set; }
  }

  class Field {

    public Field (PropertyInfo p) {
      Name = p.Name;
      var validationList = from Attribute a in p.GetCustomAttributes()
                           select mapAttributeToValidation(a);
      Validations = validationList.ToList<Validation>();

      Attributes = p.GetCustomAttributes().ToList();
      
    }
    public string Name { get; set; }

    public List<Validation> Validations { get; set; }
    [JsonProperty( NullValueHandling = NullValueHandling.Ignore )]
    public List<Attribute> Attributes { get; set; }

    Validation mapAttributeToValidation(Attribute a) {
      return new Validation(a);
    }
  }

  class Form {

    public List<Field> Fields { get; set; }

    public Form(Type viewModelType) {
      
      var fieldList = from PropertyInfo p in viewModelType.GetProperties()
                      select mapFormProperty(p);
      Fields = fieldList.ToList<Field>();
    }

    Field mapFormProperty(PropertyInfo p) {
      return new Field(p);
    }

  }

  public static class Renderer {
    public static string Form(Type viewModelType) {

      JsonSerializerSettings settings = new JsonSerializerSettings();
      settings.ContractResolver = new CustomResolver();
      settings.NullValueHandling = NullValueHandling.Ignore;

      Form f = new Form(viewModelType);
      return JsonConvert.SerializeObject(f, settings);
    }
  }
}
