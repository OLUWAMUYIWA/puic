
using System.Text.RegularExpressions;

namespace puic {
    public class Program {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome!");
            var start = true;
            while (start) {
                Console.WriteLine("Would you like to search for a holiday?");
                var cont = Console.ReadLine();
                try {
                     start = CheckContinue(cont ?? "");
                     if (!start) {
                         Console.WriteLine("Goodbye!");
                         Environment.Exit(0);
                     } else {
                         var logged = Login();
                         if (logged) {
                            var dests =  new List<string>(){"Nigeria", "Argentina", "Canada", "Israel", "Mexico", "Germany", "Japan", "Italy", "Spain", "France", "India", "Ukraine", "Poland", "Britain", "South Africa", "Pakistan"};
                            Console.WriteLine("Choose from the following destinations:");
                            
                            var count = 1;
                            foreach (var dest in dests)
                            {
                                Console.WriteLine($"{count}. {dest}");
                                count++;
                            }
                            Console.WriteLine("Give the number that corresponds to the destination of your choice");
                            int choice = 0;
                            try {
                                var parsed = Int32.TryParse(Console.ReadLine(), out choice);
                            }
                            catch (Exception e) {
                                Console.WriteLine($"Error: {e.Message}");
                            }
                            if (choice < 1 || choice > dests.Count()) {
                                Console.WriteLine("Invalid input!");
                            }
                            var chosen = dests.ElementAt(count + 1);
                            Console.WriteLine($"You have selected {chosen}. Now choose the dates");
                            Console.WriteLine("Choose beginning date in the following format: 'dd/mm/yyyy' or 'dd-mm-yyyy'");
                            var beginStr = Console.ReadLine();
                            var beginFormat = GetDateFormat(beginStr ?? "");
                            var beginDate = ParseDate(beginStr ?? "", beginFormat);
                            Console.WriteLine("Choose ending date in the following format: 'dd/mm/yyyy' or 'dd-mm-yyyy'");
                            var endStr = Console.ReadLine();
                            var endFormat = GetDateFormat(endStr ?? "");
                            var endDate = ParseDate(endStr ?? "", endFormat);
                            Package package;
                            try {
                                package = GetPackage(beginDate, endDate);
                                int cost = Cost(package);
                                Console.WriteLine($"You have successfully reserved a {PackageName(package)} at PUIC Holidays. Make sure you pay the stipulated sum of {cost} within 7 days");
                                Environment.Exit(0);
                            }
                            catch (Exception e) {
                                Console.WriteLine($"Error: {e.Message}");
                            }

                            
                         }
                         
                     }

                }
                catch (ArgumentException e) {
                    Console.WriteLine($"Error: {e.Message}");
                }
            }
        }

        static string PackageName(Package p) {
            string packageName;
            switch (p)
            {
                case Package.Mini:
                    packageName = "Mini";
                    break;
                case Package.AllIclusive:
                    packageName = "All-Iclusive";
                    break;
                case Package.SeniorCitizens:
                    packageName = "Senior-Citizens";
                    break;
                default: 
                    packageName = "Invalid";
                    break;
            }
            return packageName;
        }

        static Package GetPackage(DateOnly begin, DateOnly end) {
            Package package;
            switch (end.DayNumber - begin.DayNumber)
            {
                case 3:
                    package = Package.Mini;
                    break;
                case 7:
                    package = Package.AllIclusive;
                    break;
                case 14: 
                    package = Package.SeniorCitizens;
                    break;
                default:
                package = Package.Invalid;
                    throw new Exception("Invalid Package Choice. This is because you didn't choose your dates well.");
                
            }
            return package;
        }
        static int Cost (Package p)
        {
            int c;
            switch (p)
            {
                case Package.Mini:
                    c = 199;
                    break;
                case Package.AllIclusive:
                    c = 599;
                    break;
                case Package.SeniorCitizens:
                    c = 699;
                    break;
                default: //based on how we use the Cost() method, this default will never happen
                    c = 0;
                    break;
            }
            
            return c;
        }    
        static DateFormat GetDateFormat(string s) {
            s = s.Trim();
            string slashPattern  = @"^\d{2}/\d{2}/\d{4}$";
            string dashPattern = @"^\d{2}-\d{2}-\d{4}$";
            Regex srg = new Regex(slashPattern);
            Regex drg = new Regex(dashPattern);

            if (srg.IsMatch(s)) {
                return DateFormat.Slash;
            } else if (drg.IsMatch(s)) {
                return DateFormat.Dash;
            } else {
                return DateFormat.Invalid;
            }

        }
        static DateOnly ParseDate(string s, DateFormat f) {
            DateOnly date;
            if (f == DateFormat.Slash) {
                date = DateOnly.ParseExact(s, "dd/mm/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            } else if (f == DateFormat.Dash) {
                date = DateOnly.ParseExact(s, "dd-mm-yyyy", System.Globalization.CultureInfo.InvariantCulture);
            } else {
                throw new ArgumentException("Wrong format. check again");
            }
            return date;
        }
        static bool Login() {
            try {
                Console.WriteLine("Enter your username: ");
                var uname = Console.ReadLine();
                Console.WriteLine("Enter your passowrd: ");
                var password = Console.ReadLine();
                var unameValid = false;
                var psswdValid = false;
                if ((Normalize(uname ?? "").Length == 8) && (Normalize(uname ?? "").All(char.IsLetter))) {
                    unameValid = true;
                }
                int psswd;
                if(Int32.TryParse(Normalize(password ?? "") ?? "", out psswd) && Normalize(password ?? "").Length == 6) {
                    psswdValid = true;
                }

                if (unameValid && psswdValid) {
                    return true;
                } else {
                    return false;
                }
            }
            catch (Exception e) {
                Console.WriteLine($"Error: {e.Message}");
                return false; //comeback
            }
            
        } 
        static bool CheckContinue(string s) {
            string yes = "Y";
            string no = "N";
            if (Normalize(s) == yes) {
                return true;
            } else if (Normalize(s) == no) {
                return false;
            } else {
                throw new ArgumentException("'y' or 'n' expected");
            }
        }
        static string Normalize(string s) {
            return s.Trim().ToUpper();
        }
    } 
    public enum DateFormat {
        Slash,
        Dash,
        Invalid,
    }

    public enum Package
    {
        Mini,
        AllIclusive,
        SeniorCitizens,
        Invalid,
    }

}
