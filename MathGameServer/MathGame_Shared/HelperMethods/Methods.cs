﻿using System.Security.Cryptography;
using System.Text;

namespace MathGame_Shared.HelperMethods
{
    public static class Methods
    {
        public static string GenerateSha512Hash(string input)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha512.ComputeHash(inputBytes);

                string hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                return hashString;
            }
        }

        public static string GenerateSecurityCode(int length)
        {
            const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random(Guid.NewGuid().GetHashCode());
            char[] code = new char[length];

            for (int i = 0; i < length; i++)
            {
                code[i] = validChars[random.Next(0, validChars.Length)];
            }

            return new string(code);
        }


        public static void TrimStringProperties(this object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            var properties = obj.GetType().GetProperties();

            foreach (var property in properties)
            {

                if (property.PropertyType == typeof(string))
                {
                    var value = (string)property.GetValue(obj);

                    property.SetValue(obj, value?.Trim());
                }
            }
        }

        public static double EvaluateExpression(int operand1, int operand2, string @operator)
        {
            switch (@operator)
            {
                case "+":
                    return operand1 + operand2;
                case "-":
                    return operand1 - operand2;
                case "*":
                    return operand1 * operand2;
                case "/":
                    double result = operand1 / (double)operand2;
                    return Math.Round(result, 2); 
                default:
                    throw new ArgumentException("Invalid operator");
            }
        }
    }
}
