using System;
using System.Configuration;
using WIAM_SOS.Models;
using System.DirectoryServices.AccountManagement;
using UserAccessManagement.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Collections;

namespace WIAM_SOS.Utilities
{
    public class ActiveDirectory
    {
        private static SosDbEntities db = new SosDbEntities();
        //This method returns the Principal Context using Admin user and password.
        private static PrincipalContext getPrincipalContext()
        {
            string AdminUserName = ConfigurationManager.AppSettings["ADUserName"];
            string AdminUserPassword = ConfigurationManager.AppSettings["ADUserPassword"];
            string stringDomainName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            string OUstring = ConfigurationManager.AppSettings["ActiveDirectoryOU"];
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain, stringDomainName, OUstring, AdminUserName, AdminUserPassword);
            
            return ctx;
        }
        private static PrincipalContext getPrincipalContextForUserGroup()
        {
            string AdminUserName = ConfigurationManager.AppSettings["ADUserName"];
            string AdminUserPassword = ConfigurationManager.AppSettings["ADUserPassword"];
            string stringDomainName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            string OUstring = ConfigurationManager.AppSettings["ActiveDirectoryOUForUserGroup"];
            PrincipalContext ctx = new PrincipalContext(ContextType.Domain, stringDomainName, OUstring, AdminUserName, AdminUserPassword);
            return ctx;
        }

        public static UserPrincipal GetUser(string sUserName)
        {
            PrincipalContext oPrincipalContext = getPrincipalContext();

            UserPrincipal oUserPrincipal =
               UserPrincipal.FindByIdentity(oPrincipalContext, sUserName);
            return oUserPrincipal;
        }
        public static GroupPrincipal GetGroup(string sGroupName)
        {
            PrincipalContext oPrincipalContext = getPrincipalContextForUserGroup();
            
            GroupPrincipal oGroupPrincipal =
               GroupPrincipal.FindByIdentity(oPrincipalContext, IdentityType.Name, sGroupName);
            return oGroupPrincipal;
        }
        public static ArrayList GetUserGroups(string sUserName)
        {
            ArrayList myItems = new ArrayList();
            UserPrincipal oUserPrincipal = GetUser(sUserName);

            PrincipalSearchResult<Principal> oPrincipalSearchResult = oUserPrincipal.GetGroups();

            foreach (Principal oResult in oPrincipalSearchResult)
            {
                myItems.Add(oResult.Name);
            }
            return myItems;
        }

        public static bool CheckGroupExistence(string GroupName)
        {
            try
            {
                GroupPrincipal oGroupPrincipal = GetGroup(GroupName);

                if (oGroupPrincipal != null)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static AuthenticationResult AddUserToGroup(string sUserName, string sGroupName,string ProjectEnvironment)
        {
            try
            {
                UserPrincipal oUserPrincipal = GetUser(sUserName);
                GroupPrincipal oGroupPrincipal = GetGroup(sGroupName);
                
                //if (oUserPrincipal != null && oGroupPrincipal != null)
                {
                    if (!oGroupPrincipal.Members.Contains(oUserPrincipal))
                    {
                        oGroupPrincipal.Members.Add(oUserPrincipal);
                        oGroupPrincipal.Save();
                    }
                }
                return new AuthenticationResult();
            }
            catch(Exception ex)
            {
                return new AuthenticationResult(ex.Message);
            }
        }
        public static AuthenticationResult RemoveUserFromGroup(string sUserName, string sGroupName, string ProjectEnvironment)
        {
            try
            {
                UserPrincipal oUserPrincipal = GetUser(sUserName);
                GroupPrincipal oGroupPrincipal = GetGroup(sGroupName);
                //if (oUserPrincipal != null && oGroupPrincipal != null)
                {
                    if (oGroupPrincipal.Members.Contains(oUserPrincipal))
                    {
                        oGroupPrincipal.Members.Remove(oUserPrincipal);
                        oGroupPrincipal.Save();
                    }
                }
                return new AuthenticationResult();
            }
            catch(Exception ex)
            {
                return new AuthenticationResult(ex.Message);
            }
        }
        public static bool CheckUserExistInAD(string Email, string ProjectEnvironment)
        {
            //ADExists means AD account is there with desired EUG
            string ProjectName = ConfigurationManager.AppSettings["ProjectName"];
            string EUG = ProjectName + " " + ProjectEnvironment + " Users";
            UserPrincipal userPrincipal = GetUser(Email);
            bool IsMember = true;
            // when ProjectENv is not provided dont check EUG member. This is for case of Update when we need to check NewEMail is available for use. 
            if (!string.IsNullOrEmpty(ProjectEnvironment))
            {
                IsMember = IsUserMemberOfEUG(userPrincipal, EUG);
            }
            if (userPrincipal != null && IsMember)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }
        public static bool IsUserMemberOfProd(string sUserName)
        {
            string ProjectName = ConfigurationManager.AppSettings["ProjectName"];
            string ProdEUG = ProjectName + " PROD Users";
            UserPrincipal oUserPrincipal = GetUser(sUserName);
            GroupPrincipal oGroupPrincipal = GetGroup(ProdEUG);
            if (oUserPrincipal != null && oGroupPrincipal != null)
            {
                if (oGroupPrincipal.Members.Contains(oUserPrincipal))
                    return true;
                else
                    return false;
            }
            return false;
        }
        public static bool IsUserMemberOfEUG(UserPrincipal oUserPrincipal, string sGroupName)
        {
            GroupPrincipal oGroupPrincipal = GetGroup(sGroupName);
            if (oUserPrincipal != null && oGroupPrincipal != null)
            {
                if (oGroupPrincipal.Members.Contains(oUserPrincipal))
                    return true;
                else
                    return false;
            }
            return false;
        }
        public static bool CheckUserEnabled(string Email)
        {
            // PrincipalContext pc = getPrincipalContext();
            //UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(pc, Email);
            UserPrincipal userPrincipal = GetUser(Email);
            if (Convert.ToBoolean(userPrincipal.Enabled))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static AuthenticationResult CreateUser(string Email, string ProjectEnviournment, int? CompanyId, string Identifier, string LoggedInUserId, string Comments, string Password,string SOSUserStatus)
        {
            string ProjectName = ConfigurationManager.AppSettings["ProjectName"];
            string EUG = ProjectName + " " + ProjectEnviournment + " Users";
            ADModel ADmodel = new ADModel();
            ADmodel.Email = Email;
            if (string.IsNullOrEmpty(Password))
            {
                //Add Password as per project enviournment
                switch (ProjectEnviournment.ToUpper())
                {
                    case "PROD":
                        RandomPassword pwd = new RandomPassword();
                        string randompwd = pwd.Generate();
                        ADmodel.Password = randompwd;//ConfigurationManager.AppSettings["DefaultUserPassword"];
                        break;
                    default:
                        ADmodel.Password = ConfigurationManager.AppSettings["DefaultUserPassword"];
                        break;
                }
            }
            PrincipalContext pc = getPrincipalContext();
            UserPrincipal userPrincipal = new UserPrincipal(pc);
            string user = ADmodel.Email;
            UserPrincipal up = UserPrincipal.FindByIdentity(pc, user);
            
            if (up == null)//that means this user not found in AD
            {
                try
                {
                    if (user.Length > 20)
                    {
                        string substr = user.Substring(0, 20);
                        char[] charsToTrim = { '@', '.' };
                        substr = substr.TrimEnd(charsToTrim);
                        userPrincipal.SamAccountName = substr;
                    }
                    else
                    {
                        userPrincipal.SamAccountName = user;
                    }
                    userPrincipal.UserPrincipalName = user;
                    userPrincipal.SetPassword(ADmodel.Password);
                    userPrincipal.PasswordNeverExpires = true;
                    userPrincipal.Enabled = true;
                    userPrincipal.Save();
                    //need to send pwd in separate mail
                    //send Email to give id and password
                    //Get receiver Name based on Project Env
                    if (!string.IsNullOrEmpty(Identifier) && CompanyId.HasValue)
                    {
                        var ReceiverEmail = db.Database.SqlQuery<string>("select dbo.FnGetSenderEmail({0},{1})", ProjectEnviournment, ADmodel.Email).FirstOrDefault();
                        LEmailTemplate EmailTemplate = null;
                        if (!string.IsNullOrEmpty(SOSUserStatus) && (SOSUserStatus.Equals("Completed") || SOSUserStatus.Equals("Suspended")))
                        {//WHen user is Approved in SOS
                            EmailTemplate = db.LEmailTemplates.Where(p => p.LetTemplateName == "WelcomeEmail1Approved").Where(p => p.LetCompanyId == CompanyId).FirstOrDefault();
                        }
                        else//WHen user is not approved or not in SOS
                        {
                            EmailTemplate = db.LEmailTemplates.Where(p => p.LetTemplateName == "WelcomeEmail1NonApproved").Where(p => p.LetCompanyId == CompanyId).FirstOrDefault();
                        }

                        
                        //EmailTemplate = db.LEmailTemplates.Where(p => p.LetTemplateName == "WelcomeUser").Where(p => p.LetCompanyId == CompanyId).FirstOrDefault();
                        if (EmailTemplate != null)
                        {
                            var EmailBody = EmailTemplate.LetEmailBody;
                            EmailBody = EmailBody.Replace("###EmailAddress###", ADmodel.Email);
                            var EmailSubject = EmailTemplate.LetEmailSubject;
                            db.SpLogEmail(ReceiverEmail, null, null, null, EmailSubject, EmailBody + "<br>" + Comments, true, "Notifier", "Normal", null, "InQueue", null, LoggedInUserId, LoggedInUserId, "Test Vodafone Lite SES", "", "", "");
                        }
                        db.SpLogEmail(ReceiverEmail, null, null, null, "Vodafone LITE", "Hi ,<br>Your Vodafone LITE Password is " + ADmodel.Password, true, "Notifier", "Normal", null, "InQueue", null, LoggedInUserId, LoggedInUserId, "Test Vodafone Lite SES", null, null, null);
                    }
                    //Add User to Group
                    var result = AddUserToGroup(user, EUG, ProjectEnviournment);
                    if (!result.IsSuccess)
                    {
                        //User could not be added to EUG
                        return new AuthenticationResult(result.ErrorMessage);
                    }

                    return new AuthenticationResult();
                }
                catch (Exception ex)
                {
                    return new AuthenticationResult(ex.Message);
                }
            }
            else
            {
                //chcek user is Member of EUG
                if (IsUserMemberOfEUG(up, EUG))
                {
                    return new AuthenticationResult("User Already exists.");
                }
                else
                {
                    //Add User to Group and Enable(User might be disabled from TEST)
                    up.Enabled = true;
                    up.Save();
                    var result = AddUserToGroup(user, EUG, ProjectEnviournment);
                    if (!result.IsSuccess)
                    {
                        //User could not be added to EUG
                        return new AuthenticationResult(result.ErrorMessage);
                    }
                    //and reset pwd if PROD
                    if (ProjectEnviournment.Equals("PROD", StringComparison.OrdinalIgnoreCase))
                    {
                        ADmodel.NewPassword = ADmodel.Password; ;
                        SetUserPassword(ADmodel);
                        //Send Email After pwd Reset
                        if (!string.IsNullOrEmpty(Identifier) && CompanyId.HasValue)
                        {
                            var ReceiverEmail = db.Database.SqlQuery<string>("select dbo.FnGetSenderEmail({0},{1})", ProjectEnviournment, ADmodel.Email).FirstOrDefault();
                            var EmailTemplate = db.LEmailTemplates.Where(p => p.LetTemplateName == "WelcomeUser").Where(p => p.LetCompanyId == CompanyId).FirstOrDefault();
                            if (EmailTemplate != null)
                            {
                                var EmailBody = EmailTemplate.LetEmailBody;
                                EmailBody = EmailBody.Replace("###EmailAddress###", ADmodel.Email);
                                var EmailSubject = EmailTemplate.LetEmailSubject;
                                db.SpLogEmail(ReceiverEmail, null, null, null, EmailSubject, EmailBody + "<br>" + Comments, true, "Notifier", "Normal", null, "InQueue", null, LoggedInUserId, LoggedInUserId, "Test Vodafone Lite SES", "", "", "");
                            }
                            db.SpLogEmail(ReceiverEmail, null, null, null, "Vodafone LITE", "Hi ,<br>Your Vodafone LITE Password is " + ADmodel.Password, true, "Notifier", "Normal", null, "InQueue", null, LoggedInUserId, LoggedInUserId, "Test Vodafone Lite SES", null, null, null);
                        }
                    }
                    return new AuthenticationResult();
                }
            }
        }
        //Set password for user
        public static AuthenticationResult SetUserPassword(ADModel model)
        {
            try
            {
                PrincipalContext pc = getPrincipalContext();
                UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(pc, model.Email);
                if (userPrincipal != null)
                {
                    userPrincipal.SetPassword(model.NewPassword);
                    userPrincipal.PasswordNeverExpires = true;
                    userPrincipal.Save();
                    //password set successfully
                    return new AuthenticationResult();
                }
                else
                {
                    return new AuthenticationResult("User not found");
                }
            }
            catch (Exception ex)
            {
                return new AuthenticationResult(ex.Message);
            }
        }

        //Update only User email. This is used in Update Call
        public static AuthenticationResult UpdateUser(string OldEmail,string NewEmail, string ProjectEnviournment)
        {
            string ProjectName = ConfigurationManager.AppSettings["ProjectName"];
            string EUG = ProjectName + " " + ProjectEnviournment + " Users";
            string ProdEUG = ProjectName + " PROD Users";

            PrincipalContext pc = getPrincipalContext();
            UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(pc, OldEmail);
            if (userPrincipal != null)
            {
                if (ProjectEnviournment.Equals("PROD", StringComparison.OrdinalIgnoreCase))
                {
                    userPrincipal.UserPrincipalName  = NewEmail;
                    userPrincipal.Save();
                }
                else if (!IsUserMemberOfEUG(userPrincipal, ProdEUG))
                {
                    userPrincipal.UserPrincipalName = NewEmail;
                    userPrincipal.Save();
                }
                else
                {
                    return new AuthenticationResult("AD Account cannot be updated as this is PROD account.");
                }
                return new AuthenticationResult();
            }
            else
            {
                return new AuthenticationResult("AD Account does not exist");
            }

        }
       
        //This method is used to Activate existing user on Enable call. 
        public static AuthenticationResult ActivateUser(string Email,string ProjectEnviournment)
        {
            string ProjectName = ConfigurationManager.AppSettings["ProjectName"];
            string EUG = ProjectName + " " + ProjectEnviournment + " Users";
            string ProdEUG = ProjectName + " PROD Users";

            PrincipalContext pc = getPrincipalContext();
            UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(pc, Email);
            if (userPrincipal != null)
            {
                if (ProjectEnviournment.Equals("PROD", StringComparison.OrdinalIgnoreCase))
                {
                    userPrincipal.Enabled = true;
                    userPrincipal.Save();
                }
                else if(!IsUserMemberOfEUG(userPrincipal, ProdEUG))
                {
                    userPrincipal.Enabled = true;
                    userPrincipal.Save();
                }
                else
                {
                    return new AuthenticationResult("AD Account cannot be enabled as this is PROD account.");
                }
                return new AuthenticationResult();
            }
            else
            {
                return new AuthenticationResult("AD Account does not exist");
            }
            
        }
        //This method is used to DaActivate existing user on Disable call. 
        public static AuthenticationResult DeactivateUser(string Email, string ProjectEnvironment)
        {
            string ProjectName = ConfigurationManager.AppSettings["ProjectName"];
            string EUG = ProjectName + " " + ProjectEnvironment + " Users";
            string ProdEUG = ProjectName + " PROD Users";

            PrincipalContext pc = getPrincipalContext();
            UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(pc, Email);
            if (userPrincipal != null)
            {
                if (ProjectEnvironment.Equals("PROD", StringComparison.OrdinalIgnoreCase))
                {
                    userPrincipal.Enabled = false;
                    userPrincipal.Save();
                }
                else if (!IsUserMemberOfEUG(userPrincipal, ProdEUG))//if not PROD user
                {
                    userPrincipal.Enabled = false;
                    userPrincipal.Save();
                }
                else
                {
                    return new AuthenticationResult("AD Account cannot be disabled as this is PROD account.");
                }
                return new AuthenticationResult();
            }
            else
            {
                return new AuthenticationResult("AD Account does not exist");
            }
        }
        //Delete an existing user ,This method will be used on Revoke Call
        //Delete an existing user 
        public static AuthenticationResult DeleteUser(string Email, string ProjectEnviournment)
        {
            string ProjectName = ConfigurationManager.AppSettings["ProjectName"];
            string EUG = ProjectName + " " + ProjectEnviournment + " Users";
            string ProdEUG = ProjectName + " PROD Users";
            try
            {
                PrincipalContext pc = getPrincipalContext();
                UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(pc, Email);
                if (userPrincipal != null)
                {
                    //Delete User only when request Env is Prod. 
                    if (ProjectEnviournment.Equals("Prod", StringComparison.OrdinalIgnoreCase))
                    {
                        userPrincipal.Delete();//deleted successfully
                    }
                    else if (!IsUserMemberOfEUG(userPrincipal,ProdEUG))//not Prod user
                    {
                        userPrincipal.Delete();//deleted successfully
                    }
                    else //remove user from EUG
                    {
                        RemoveUserFromGroup(Email, EUG,ProjectEnviournment);
                    }
                }
                else
                {
                    return new AuthenticationResult("user not found");
                }
                return new AuthenticationResult();

            }
            catch (Exception ex)
            {
                return new AuthenticationResult(ex.Message);
            }
        }
        
        public partial class AuthenticationResult
        {
            public AuthenticationResult(string errorMessage = null)
            {
                ErrorMessage = errorMessage;
            }

            public String ErrorMessage { get; private set; }
            public Boolean IsSuccess => String.IsNullOrEmpty(ErrorMessage);
        }

        public class RandomPassword
        {
            // Define default min and max password lengths.
            private static int DEFAULT_MIN_PASSWORD_LENGTH = 8;
            private static int DEFAULT_MAX_PASSWORD_LENGTH = 10;

            // Define supported password characters divided into groups.
            // You can add (or remove) characters to (from) these groups.
            private static string PASSWORD_CHARS_LCASE = "abcdefgijkmnpqrstwxyz";
            private static string PASSWORD_CHARS_UCASE = "ABCDEFGHJKLMNPQRSTWXYZ";
            private static string PASSWORD_CHARS_NUMERIC = "23456789";
            //private static string PASSWORD_CHARS_SPECIAL = "*$-+?_&=!%{}/";
            public string Generate()
            {
                return Generate(DEFAULT_MIN_PASSWORD_LENGTH,
                                DEFAULT_MAX_PASSWORD_LENGTH);
            }
            public string Generate(int length)
            {
                return Generate(length, length);
            }
            public string Generate(int minLength,
                                          int maxLength)
            {
                // Make sure that input parameters are valid.
                if (minLength <= 0 || maxLength <= 0 || minLength > maxLength)
                    return null;

                // Create a local array containing supported password characters
                // grouped by types. You can remove character groups from this
                // array, but doing so will weaken the password strength.
                char[][] charGroups = new char[][]
                {
            PASSWORD_CHARS_LCASE.ToCharArray(),
            PASSWORD_CHARS_UCASE.ToCharArray(),
            PASSWORD_CHARS_NUMERIC.ToCharArray(),
                    //PASSWORD_CHARS_SPECIAL.ToCharArray()
                };

                // Use this array to track the number of unused characters in each
                // character group.
                int[] charsLeftInGroup = new int[charGroups.Length];

                // Initially, all characters in each group are not used.
                for (int i = 0; i < charsLeftInGroup.Length; i++)
                    charsLeftInGroup[i] = charGroups[i].Length;

                // Use this array to track (iterate through) unused character groups.
                int[] leftGroupsOrder = new int[charGroups.Length];

                // Initially, all character groups are not used.
                for (int i = 0; i < leftGroupsOrder.Length; i++)
                    leftGroupsOrder[i] = i;

                // Because we cannot use the default randomizer, which is based on the
                // current time (it will produce the same "random" number within a
                // second), we will use a random number generator to seed the
                // randomizer.

                // Use a 4-byte array to fill it with random bytes and convert it then
                // to an integer value.
                byte[] randomBytes = new byte[4];

                // Generate 4 random bytes.
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                rng.GetBytes(randomBytes);

                // Convert 4 bytes into a 32-bit integer value.
                int seed = BitConverter.ToInt32(randomBytes, 0);

                // Now, this is real randomization.
                Random random = new Random(seed);

                // This array will hold password characters.
                char[] password = null;

                // Allocate appropriate memory for the password.
                if (minLength < maxLength)
                    password = new char[random.Next(minLength, maxLength + 1)];
                else
                    password = new char[minLength];

                // Index of the next character to be added to password.
                int nextCharIdx;

                // Index of the next character group to be processed.
                int nextGroupIdx;

                // Index which will be used to track not processed character groups.
                int nextLeftGroupsOrderIdx;

                // Index of the last non-processed character in a group.
                int lastCharIdx;

                // Index of the last non-processed group.
                int lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;

                // Generate password characters one at a time.
                for (int i = 0; i < password.Length; i++)
                {
                    // If only one character group remained unprocessed, process it;
                    // otherwise, pick a random character group from the unprocessed
                    // group list. To allow a special character to appear in the
                    // first position, increment the second parameter of the Next
                    // function call by one, i.e. lastLeftGroupsOrderIdx + 1.
                    if (lastLeftGroupsOrderIdx == 0)
                        nextLeftGroupsOrderIdx = 0;
                    else
                        nextLeftGroupsOrderIdx = random.Next(0,
                                                             lastLeftGroupsOrderIdx);

                    // Get the actual index of the character group, from which we will
                    // pick the next character.
                    nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx];

                    // Get the index of the last unprocessed characters in this group.
                    lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1;

                    // If only one unprocessed character is left, pick it; otherwise,
                    // get a random character from the unused character list.
                    if (lastCharIdx == 0)
                        nextCharIdx = 0;
                    else
                        nextCharIdx = random.Next(0, lastCharIdx + 1);

                    // Add this character to the password.
                    password[i] = charGroups[nextGroupIdx][nextCharIdx];

                    // If we processed the last character in this group, start over.
                    if (lastCharIdx == 0)
                        charsLeftInGroup[nextGroupIdx] =
                                                  charGroups[nextGroupIdx].Length;
                    // There are more unprocessed characters left.
                    else
                    {
                        // Swap processed character with the last unprocessed character
                        // so that we don't pick it until we process all characters in
                        // this group.
                        if (lastCharIdx != nextCharIdx)
                        {
                            char temp = charGroups[nextGroupIdx][lastCharIdx];
                            charGroups[nextGroupIdx][lastCharIdx] =
                                        charGroups[nextGroupIdx][nextCharIdx];
                            charGroups[nextGroupIdx][nextCharIdx] = temp;
                        }
                        // Decrement the number of unprocessed characters in
                        // this group.
                        charsLeftInGroup[nextGroupIdx]--;
                    }

                    // If we processed the last group, start all over.
                    if (lastLeftGroupsOrderIdx == 0)
                        lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
                    // There are more unprocessed groups left.
                    else
                    {
                        // Swap processed group with the last unprocessed group
                        // so that we don't pick it until we process all groups.
                        if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
                        {
                            int temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                            leftGroupsOrder[lastLeftGroupsOrderIdx] =
                                        leftGroupsOrder[nextLeftGroupsOrderIdx];
                            leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                        }
                        // Decrement the number of unprocessed groups.
                        lastLeftGroupsOrderIdx--;
                    }
                }

                // Convert password characters into a string and return the result.
                //SS Adding # at the end
                String Password = new string(password);
                //return Password+"#";
                return Password;
            }
        }
    }
}