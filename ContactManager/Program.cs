using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ContactManager
{
    [Serializable]
    public class Contact
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public int PhoneNumber { get; set; }
        public string Address { get; set; }

        public override string ToString()
        {
            // Join
            return "\n" + $"Contact: {Name}, {LastName}, {PhoneNumber}, {Address}";
        }

        public static void Serialize(string path, List<Contact> list)
        {
            using (Stream stream = File.Open(path, FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, list);
            }
        }

        // out vietoj list. Iskelti i ContactRepository ir pasidaryt private
        public static void Deserialize(string path, List<Contact> list)
        {
            using (Stream stream = File.Open(path, FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                list = (List<Contact>)binaryFormatter.Deserialize(stream);
            }
        }

        public static List<Contact> DeserializeForAddition(string path)
        {
            using (var stream = File.Open(path, FileMode.OpenOrCreate))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return stream.Length == 0 ?
                    new List<Contact>() :
                    (List<Contact>)binaryFormatter.Deserialize(stream);
                
               
            }
        }



    }

    public class ContactRepository
    {

        const string path = @"./contact.txt";

        // Cheks if Unique vardas - pakeisti
        public static bool CheckUniqueness(List<Contact> list, int number)
        {
            bool hasNumber = list.Any(val => val.PhoneNumber == number);
            return hasNumber;
        }


        // IsValidNumber - pakeisti varda
        public static int CheckIfNumber(string phoneNumberAsString)
        {
            int input_phoneNumber;
            while (!int.TryParse(phoneNumberAsString, out input_phoneNumber))
            {
                Console.WriteLine("Not a number. Try again.");
                phoneNumberAsString = Console.ReadLine();
            }
            return input_phoneNumber;
        }


        // ISMESTI
        public static int CheckExistance(List<Contact> list, Contact result)
        {
            int input_phoneNumber;
            string phoneNumberAsString;
            bool ifExists = list.Contains(result);
            while (ifExists != list.Contains(result))
            {
                Console.WriteLine("This number is not present in list. Please enter new one.");
            }
            phoneNumberAsString = Console.ReadLine();
            input_phoneNumber = CheckIfNumber(phoneNumberAsString);
            return input_phoneNumber;
        }


        // Paprasti metodai - jokios logikos.
        // Add metodas - prideti kontakta
        public static void AddContact()
        {

            List<Contact> existingContacts;

            // Deserialize metodas
            existingContacts = Contact.DeserializeForAddition(path);

            Console.WriteLine("-- Enter your Name: ", "\n");
            var input_name = Console.ReadLine();

            Console.WriteLine("-- Enter your Last Name: ", "\n");
            var input_lastName = Console.ReadLine();

            Console.WriteLine("-- Enter your phone number: ", "\n");
            string phoneNumberAsString = Console.ReadLine();


            // Patikrinimai - ar skaicius, ar unikalus

            int input_phoneNumber = CheckIfNumber(phoneNumberAsString);
            Console.WriteLine(input_phoneNumber);
            bool check = CheckUniqueness(existingContacts, input_phoneNumber);

            if (check == true)
            {
                Console.WriteLine("Number already exists. Please enter new one.");
                phoneNumberAsString = Console.ReadLine();
                input_phoneNumber = CheckIfNumber(phoneNumberAsString);
            }

            Console.WriteLine("-- Enter your address: ", "\n");
            var input_address = Console.ReadLine();

            // Objekto sukurimas 
            var contactToAdd = new Contact
            {
                Name = input_name,
                LastName = input_lastName,
                PhoneNumber = input_phoneNumber,
                Address = input_address
            };


            // Pridedame kontakta i lista
            existingContacts.Add(contactToAdd);

            // Iškviecamas Serialize metodas
            Contact.Serialize(path, existingContacts);

        }

        // Update metodas
        public static void UpdateContact()
        {

            // 1 - Display full list of contacts to choose ----------------------------------------------------------------

            Console.WriteLine("-- Full list of contacts: ");

            List<Contact> existingContacts;

            using (var stream = File.Open(path, FileMode.OpenOrCreate))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                existingContacts = stream.Length == 0 ?
                    new List<Contact>() :
                    (List<Contact>)binaryFormatter.Deserialize(stream);
            }

            var pattern = string.Join(",", existingContacts.Select(cff => cff.ToString()));
            Console.WriteLine(pattern);

            // 2 - Enter contact by number which want to be edited ----------------------------------------------------------------

            Console.WriteLine("-- Enter editable contact number: ");

            string phoneNumberAsString = Console.ReadLine();


            int input_phoneNumber = CheckIfNumber(phoneNumberAsString);

            // 3- Find a number. ----------------------------------------------------------------------------------------
            var result = existingContacts.Find(x => x.PhoneNumber == input_phoneNumber);
            Console.WriteLine(result);

            while (result == null)
            {
                Console.WriteLine("This number is not present in list. Please enter new one.");
                phoneNumberAsString = Console.ReadLine();
                input_phoneNumber = CheckIfNumber(phoneNumberAsString);
                result = existingContacts.Find(x => x.PhoneNumber == input_phoneNumber);
                Console.WriteLine(result);

            }

            //4 -  Select what to update ------------------------------------------------------------------------------ 
            //Console.WriteLine("-- Selected object is: " + "\n" + result);
            Console.WriteLine("--- Select confirmed. Select what to update: ", "\n");

            string mode;
            string retry = "No";

            do
            {
                Console.WriteLine("\n");
                Console.WriteLine("---Select Mode: " + "\n" + "1 - Name" + "\n" + "2 - Last Name" + "\n" + "3 - Phone number" + "\n" + "4 - Address");
                mode = Console.ReadLine();
                switch (mode)
                {
                    case "1":
                        Console.WriteLine("-----------------------------", "\n");
                        Console.WriteLine("Selected - Update Name", "\n");
                        Console.WriteLine("-----------------------------", "\n");

                        // Vardas bus keiciama
                        Console.WriteLine("-- Enter New Name: ", "\n");
                        var input_name = Console.ReadLine();
                        result.Name = input_name;

                        // Irasymas ir parodymas
                        Contact.Serialize(path, existingContacts);
                        ContactRepository.ViewContacts();

                        Console.WriteLine("Update is finished or there are no such mode available. Do you want to select another mode? (Yes/No)");
                        retry = Console.ReadLine();
                        break;

                    case "2":
                        Console.WriteLine("-----------------------------", "\n");
                        Console.WriteLine("Selected - Update Last Name");
                        Console.WriteLine("-----------------------------", "\n");

                        // Pavarde bus keiciama
                        Console.WriteLine("-- Enter New Last Name: ", "\n");
                        var input_lastName = Console.ReadLine();
                        result.LastName = input_lastName;

                        // Irasymas ir parodymas
                        Contact.Serialize(path, existingContacts);
                        ContactRepository.ViewContacts();

                        Console.WriteLine("Update is finished or there are no such mode available. Do you want to select another mode? (Yes/No)");
                        retry = Console.ReadLine();
                        break;

                    case "3":
                        Console.WriteLine("-----------------------------", "\n");
                        Console.WriteLine("Selected - Update Phone Number");
                        Console.WriteLine("-----------------------------", "\n");

                        // Numeris bus keiciamas
                        Console.WriteLine("-- Enter New Phone Number Name: ", "\n");
                        phoneNumberAsString = Console.ReadLine();

                        // Patikrinimas 
                        input_phoneNumber = CheckIfNumber(phoneNumberAsString);
                        Console.WriteLine(input_phoneNumber);
                        bool check = CheckUniqueness(existingContacts, input_phoneNumber);
                        Console.WriteLine(check);
                        if (check == true)
                        {
                            Console.WriteLine("Number already exists. Please enter new one.");
                            phoneNumberAsString = Console.ReadLine();
                            input_phoneNumber = CheckIfNumber(phoneNumberAsString);
                        }
                        result.PhoneNumber = input_phoneNumber;

                        // Irasymas ir parodymas
                        Contact.Serialize(path, existingContacts);
                        ContactRepository.ViewContacts();

                        Console.WriteLine("Update is finished or there are no such mode available. Do you want to select another mode? (Yes/No)");
                        retry = Console.ReadLine();
                        break;

                    case "4":
                        Console.WriteLine("-----------------------------", "\n");
                        Console.WriteLine("Selected - Update Address", "\n");
                        Console.WriteLine("-----------------------------", "\n");

                        // Adresas bus keiciama
                        Console.WriteLine("-- Enter New Last Name: ", "\n");
                        var input_address = Console.ReadLine();
                        result.Address = input_address;

                        // Irasymas ir parodymas
                        Contact.Serialize(path, existingContacts);
                        ContactRepository.ViewContacts();

                        Console.WriteLine("Update is finished or there are no such mode available. Do you want to select another mode? (Yes/No)");
                        retry = Console.ReadLine();
                        break;

                    default:
                        Console.WriteLine("Update is finished or there are no such mode available. Do you want to select another mode? (Yes/No)");
                        retry = Console.ReadLine();
                        break;
                }
            }
            while (retry != "No");
        }

        // Delete metodas
        public static void DeleteContact()
        {
            // 1 - Display full list of contacts to choose ----------------------------------------------------------------

            Console.WriteLine("-- Full list of contacts: ");

            List<Contact> existingContacts;

            using (var stream = File.Open(path, FileMode.OpenOrCreate))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                existingContacts = stream.Length == 0 ?
                    new List<Contact>() :
                    (List<Contact>)binaryFormatter.Deserialize(stream);
            }

            var pattern = string.Join(",", existingContacts.Select(cff => cff.ToString()));
            Console.WriteLine(pattern);

            // 2 - Enter contact by number which want to be deleted ----------------------------------------------------------------

            Console.WriteLine("-- Enter contact number for deletion: ");

            string phoneNumberAsString = Console.ReadLine();

            int input_phoneNumber;
            while (!int.TryParse(phoneNumberAsString, out input_phoneNumber))
            {
                Console.WriteLine("Not a number. Try again.");
                phoneNumberAsString = Console.ReadLine();
            }

            // 3- Find a number. ----------------------------------------------------------------------------------------
            var result = existingContacts.Find(x => x.PhoneNumber == input_phoneNumber);
            Console.WriteLine(result);

            while (result == null)
            {
                Console.WriteLine("This number is not present in list. Please enter new one.");
                phoneNumberAsString = Console.ReadLine();
                input_phoneNumber = CheckIfNumber(phoneNumberAsString);
                result = existingContacts.Find(x => x.PhoneNumber == input_phoneNumber);
                Console.WriteLine(result);

            }

            // Istrinamas kontaktas
            existingContacts.Remove(result);

            // Irasomas atnaujintas listas atgal i faila 
            Contact.Serialize(path, existingContacts);
        }

        // View metodas - pervadinti i Get
        public static void ViewContacts()
        {


            List<Contact> contactfromFile;

            using (Stream stream = File.Open(path, FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                contactfromFile = (List<Contact>)binaryFormatter.Deserialize(stream);
            }

            var pattern = string.Join(",", contactfromFile.Select(cff => cff.ToString()));
            Console.WriteLine(pattern);
        }
    }

    class Program
    {

        static void Main(string[] args)
        {

            string mode;
            string retry = "No";

            do
            {
                Console.WriteLine("\n");
                Console.WriteLine("---Select Mode: " + "\n" + "0 - Add" + "\n" + "1 - Update" + "\n" + "2 - Delete" + "\n" + "3 - View");
                mode = Console.ReadLine();
                switch (mode)
                {
                    case "0":
                        Console.WriteLine("-----------------------------", "\n");
                        Console.WriteLine("Selected Mode - Add a contact", "\n");
                        Console.WriteLine("-----------------------------", "\n");
                        ContactRepository.AddContact();

                        Console.WriteLine("Action is finished or there are no such mode available. Do you want to select another mode? (Yes/No)");
                        retry = Console.ReadLine();
                        break;

                    case "1":
                        Console.WriteLine("-----------------------------", "\n");
                        Console.WriteLine("Selected Mode - Update contact information");
                        Console.WriteLine("-----------------------------", "\n");
                        ContactRepository.UpdateContact();

                        Console.WriteLine("Action is finished or there are no such mode available. Do you want to select another mode? (Yes/No)");
                        retry = Console.ReadLine();
                        break;

                    case "2":
                        Console.WriteLine("-----------------------------", "\n");
                        Console.WriteLine("Selected Mode - Delete contact");
                        Console.WriteLine("-----------------------------", "\n");
                        ContactRepository.DeleteContact();

                        Console.WriteLine("Action is finished or there are no such mode available. Do you want to select another mode? (Yes/No)");
                        retry = Console.ReadLine();
                        break;

                    case "3":
                        Console.WriteLine("-----------------------------", "\n");
                        Console.WriteLine("Current contact list: ", "\n");
                        Console.WriteLine("-----------------------------", "\n");
                        ContactRepository.ViewContacts();

                        Console.WriteLine("Action is finished or there are no such mode available. Do you want to select another mode? (Yes/No)");
                        retry = Console.ReadLine();
                        break;

                    default:
                        Console.WriteLine("Action is finished or there are no such mode available. Do you want to select another mode? (Yes/No)");
                        retry = Console.ReadLine();
                        break;
                }

            }
            while (retry != "No");
        }
    }
}
