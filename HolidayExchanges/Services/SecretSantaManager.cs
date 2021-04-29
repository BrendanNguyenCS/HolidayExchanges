using HolidayExchanges.DAL;
using HolidayExchanges.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace HolidayExchanges.Services
{
    /// <summary>
    /// Represents the logic of a secret santa event
    /// </summary>
    public class SecretSantaManager
    {
        /// <value>The database context</value>
        private readonly SecretSantaDbContext _context;

        /// <summary>
        /// Instantiates a new instance of the <see cref="SecretSantaManager"/> class.
        /// </summary>
        /// <param name="context">The <see cref="SecretSantaDbContext"/> instance.</param>
        public SecretSantaManager(SecretSantaDbContext context)
        {
            _context = context;
        }

        #region Information Gathering Helpers

        /// <summary>
        /// Gets all users in group.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public List<User> GetAllUsersInGroup(int id)
        {
            if (id == 0) throw new ArgumentOutOfRangeException();
            var users = _context.UserGroups
                .Where(ug => ug.GroupID == id)
                .Select(ug => ug.User)
                .ToList();
            return users;
        }

        /// <summary>
        /// Gets all groups for user.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public List<Group> GetAllGroupsForUser(int id)
        {
            if (id == 0) throw new ArgumentOutOfRangeException();
            var groups = _context.UserGroups
                .Where(ug => ug.UserID == id)
                .Select(ug => ug.Group)
                .ToList();
            return groups;
        }

        /// <summary>
        /// Gets the user groups for user.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public List<UserGroup> GetUserGroupsForUser(int id)
        {
            if (id == 0) throw new ArgumentOutOfRangeException();
            var usergroups = _context.UserGroups
                .Where(ug => ug.UserID == id)
                .ToList();
            return usergroups;
        }

        /// <summary>
        /// Gets the user groups for group.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public List<UserGroup> GetUserGroupsForGroup(int id)
        {
            if (id == 0) throw new ArgumentOutOfRangeException();
            var usergroups = _context.UserGroups
                .Where(ug => ug.GroupID == id)
                .ToList();
            return usergroups;
        }

        /// <summary>
        /// Gets the group by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Group GetGroupById(int id)
        {
            if (id == 0) throw new ArgumentOutOfRangeException();
            var group = _context.Groups.Find(id);
            return group;
        }

        /// <summary>
        /// Gets the user by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public User GetUserById(int id)
        {
            if (id == 0) throw new ArgumentOutOfRangeException();
            var user = _context.Users.Find(id);
            return user;
        }

        /// <summary>
        /// Gets the recipients for all groups for user.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <param name="groups">The groups.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <remarks>Will mainly be used by the <see cref="ViewModels"/></remarks>
        public List<int> GetRecipientIdsForAllGroupsForUser(int userID, List<Group> groups)
        {
            var recipientList = new List<int>();
            // if user isn't in any groups, return empty list to avoid exceptions
            if (!_context.UserGroups.Any(ug => ug.UserID == userID)) return recipientList;
            foreach (var group in groups)
            {
                try
                {
                    var result = _context.UserGroups
                        .Where(ug => ug.UserID == userID)
                        .Where(ug => ug.GroupID == group.GroupID)
                        .Select(ug => ug.RecipientUserID)
                        .Single();

                    var recipient = _context.Users.Find(result);
                    recipientList.Add(result);
                }
                catch (ArgumentNullException ex)
                {
                    throw new ArgumentNullException(ex.Message);
                }
                catch (InvalidOperationException ex)
                {
                    throw new InvalidOperationException(ex.Message);
                }
            }
            return recipientList;
        }

        /// <summary>
        /// Gets the recipients for all groups for user.
        /// </summary>
        /// <param name="userID">The user identifier.</param>
        /// <param name="groups">The groups.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Looks like this user doesn't exist.</exception>
        public List<User> GetRecipientsForAllGroupsForUser(int userID, List<Group> groups)
        {
            var ids = GetRecipientIdsForAllGroupsForUser(userID, groups);
            var recipients = new List<User>();
            // if user isn't in any groups, return empty list to avoid exceptions
            if (!_context.UserGroups.Any(ug => ug.UserID == userID)) return recipients;
            foreach (var id in ids)
            {
                // hasn't been paired yet
                if (id == 0)
                {
                    recipients.Add(null);
                }
                else
                {
                    var currentPair = _context.Users.Find(id);
                    if (currentPair == null) throw new InvalidOperationException("Looks like this user doesn't exist.");
                    recipients.Add(currentPair);
                }
            }
            return recipients;
        }

        /// <summary>
        /// Given the <paramref name="userID"/> and <paramref name="groupID"/>, returns the
        /// recipient user
        /// </summary>
        /// <param name="userID">The user identifier</param>
        /// <param name="groupID">The group identifier</param>
        /// <returns></returns>
        public User GetRecipientForUserInGroup(int userID, int groupID)
        {
            int resultId = _context.UserGroups
                .Where(ug => ug.UserID == userID)
                .Where(ug => ug.GroupID == groupID)
                .Select(ug => ug.RecipientUserID)
                .Single();
            return _context.Users.Find(resultId);
        }

        #endregion Information Gathering Helpers

        /// <summary>
        /// Gets the recipient assignments.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void GetRecipientAssignments(int id)
        {
            if (id == 0) throw new ArgumentOutOfRangeException();
            var usersList = GetAllUsersInGroup(id);
            usersList = usersList.LinqShuffle();
            Pair(usersList, id);
        }

        /// <summary>
        /// Pairs the specified users.
        /// </summary>
        /// <param name="users">The users.</param>
        /// <param name="id">The identifier.</param>
        /// <exception cref="ArgumentNullException">That group doesn't exist. Please try again.</exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="InvalidOperationException">
        /// A user could not be found. or It seems that a user doesn't belong in this group.
        /// </exception>
        public void Pair(List<User> users, int id)
        {
            if (users == null) throw new ArgumentNullException("The users list doesn't exist");
            if (id == 0) throw new ArgumentOutOfRangeException();
            // access db to find current group and create local variables for scope purposes
            var currentGroup = GetGroupById(id);
            if (currentGroup == null) throw new ArgumentNullException("That group doesn't exist. Please try again.");

            for (int i = 0; i < users.Count; i++)
            {
                try
                {
                    int userId = users[i].UserID;
                    var currentUser = _context.Users
                        .Where(u => u.UserID == userId)
                        .SingleOrDefault();
                    if (currentUser == null) throw new InvalidOperationException("A user could not be found.");
                    // assign pairs
                    var userGroup = _context.UserGroups
                        .Where(ug => ug.UserID == currentUser.UserID)
                        .Where(ug => ug.GroupID == currentGroup.GroupID)
                        .SingleOrDefault();
                    if (userGroup == null) throw new InvalidOperationException("It seems that a user doesn't belong in this group.");
                    //_context.Entry(userGroup).State = EntityState.Modified;
                    _context.UserGroups.Attach(userGroup);
                    int recipientId;
                    if (i == users.Count - 1)
                    {
                        recipientId = users[0].UserID;
                    }
                    else
                    {
                        recipientId = users[i + 1].UserID;
                    }
                    userGroup.RecipientUserID = recipientId;
                    _context.SaveChanges();

                    var recipient = _context.Users.Find(recipientId);

                    #region Not Working DB Update

                    //var currentUser = userGroup.User; // gets current user from db
                    //User recipient;
                    //if (i == list.Count - 1)
                    //{
                    //    recipient = _context.Users.Find(list[0].UserID);
                    //}
                    //else
                    //{
                    //    recipient = _context.Users.Find(list[i + 1].UserID);
                    //}// gets the following adjacent user from db
                    //var userGroup = _context.UserGroups.SingleOrDefault(u => u.UserID == currentUser.UserID && u.GroupID == currentGroup.GroupID); // returns correct usergroup
                    //userGroup.RecipientUserID = recipient.UserID; // appends it to add the assignment
                    ////_context.UserGroups.AddOrUpdate(userGroup);
                    //_context.SaveChanges();

                    #endregion Not Working DB Update
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.InnerException);
                    throw;
                }
            }

            _context.Groups.Attach(currentGroup);
            currentGroup.HasBeenPaired = true;
            _context.SaveChanges();
        }

        /// <summary>
        /// Sends the match email.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="recipient">The recipient.</param>
        /// <param name="group">The group.</param>
        public void SendMatchEmail(User user, User recipient, Group group)
        {
            var message = new MailMessage();
            message.To.Add(user.Email);

            message.Subject = "Your assignment for the group " + group.Name;
            message.Body = "<h1>Secret Santa Pair Notification</h1>" + Environment.NewLine +
                "<p>Hello " + user.UserName + ",</p>" + Environment.NewLine +
                "<p>Your recipient for the " + group.Name + " group has been chosen.<p>" + Environment.NewLine +
                "<p><strong>Your recipient</strong>: " + recipient.UserName + "</p>" + Environment.NewLine +
                "<p>To view their wishlist, go to our website and view your profile.</p>" + Environment.NewLine +
                "<p>Thank you,</p>" + Environment.NewLine +
                "<p>From your friends at Holiday Exchanges</p>";
            message.IsBodyHtml = true;

            try
            {
                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.Send(message);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("{0}: {1}", ex.ToString(), ex.Message);
                throw;
            }
        }
    }
}