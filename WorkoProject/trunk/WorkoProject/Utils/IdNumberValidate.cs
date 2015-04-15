using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WorkoProject.Models
{
    public class IdNumberValidate : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var idValue = value.ToString();
            if (idValue != null)
            {
                idValue = idValue.PadLeft(9, '0');
                int[] idMul = new int[9];
                for (int i = 0; i < idValue.Length; i++)
                {
                    idMul[i] = Int32.Parse(idValue[i].ToString()) * (i % 2 == 0 ? 1 : 2);
                    if (idMul[i] > 9)
                    {
                        int n = idMul[i];
                        idMul[i] = n % 10 + n / 10;
                    }
                }

                if (idMul.Sum() % 10 != 0)
                    return new ValidationResult("Please enter valid ID number");
            }

            return ValidationResult.Success;
        }
    }
}