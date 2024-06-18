using System.ComponentModel;
using System.Windows.Forms;
using static adminAPI.API;

namespace adminAPI
{
    public partial class adminwindow : Form
    {
        private readonly MessengerClient messengerclient;
        public BindingList<MessengerClient.User> users;
        public BindingList<MessengerClient.Message> mess;
        public adminwindow()
        {
            InitializeComponent();
            messengerclient = new MessengerClient();
            LoadUsers();
            LoadMessages();
        }
        private async void add_Click(object sender, EventArgs e)
        {
            if (login.Text != "" || password.Text != "" || fio.Text != "" || post.Text != "")
            {
                await messengerclient.AddUser(login.Text, password.Text, fio.Text, post.Text);
                MessageBox.Show("Пользователь был добавлен");
                LoadUsers();
            }
            else MessageBox.Show("Заполните все поля");
        }
        private async void delete_acc_by_id_Click(object sender, EventArgs e)
        {
            if (accid.Text != "")
            {
                await messengerclient.Delete_user_by_id(int.Parse(accid.Text));
                MessageBox.Show("Пользователь был удалён");
                LoadUsers();
            }
        }

        private async void delete_all_users_Click(object sender, EventArgs e)
        {
            try
            {
                await messengerclient.DeleteAllUsersData();
                LoadUsers();
            }
            catch { }
            finally
            {
            }
        }

        private async void delete_all_messages_Click(object sender, EventArgs e)
        {
            await messengerclient.DeleteAllMessagesData();
            LoadMessages();
        }

        private async void reset_autoincrement_messages_Click(object sender, EventArgs e)
        {
            try
            {
                await messengerclient.ResetAutoIncrementMessages();
                LoadMessages();
            }
            catch { }
            finally
            {

            }
        }

        private async void reset_autoincrement_users_Click(object sender, EventArgs e)
        {
            try
            {
                await messengerclient.ResetAutoIncrementUsers();
                LoadUsers();
            }
            catch { }
            finally
            {

            }
        }

        private async void addcontact_Click(object sender, EventArgs e)
        {
            if (myId.Text != "" || contactId.Text != "")
            {
                await messengerclient.AddContact(int.Parse(myId.Text), int.Parse(contactId.Text));
                LoadUsers();
            }
        }
        private async void LoadUsers()
        {
            try
            {
                var userList = await messengerclient.GetAllUsers();
                users = new BindingList<API.MessengerClient.User>(userList);
                users_grid.DataSource = users;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading users: " + ex.Message);
            }
        }
        private async void LoadMessages()
        {
            try
            {
                var messageList = await messengerclient.GetAllMessages();
                mess = new BindingList<API.MessengerClient.Message>(messageList);
                messages_grid.DataSource = mess;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading users: " + ex.Message);
            }
        }

        private void reload_grid_Click(object sender, EventArgs e)
        {
            LoadUsers();
        }

        private void reload_messages_Click(object sender, EventArgs e)
        {
            LoadMessages();
        }

        private void users_grid_SelectionChanged(object sender, EventArgs e)
        {

        }

        private async void users_grid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                var editedUser = users_grid.Rows[e.RowIndex].DataBoundItem as MessengerClient.User;
                if (editedUser != null)
                {
                    // Здесь вы можете сохранить изменения в объекте User
                    // Например, вызвать метод для обновления данных на сервере
                    try
                    {
                        await messengerclient.UpdateUser(editedUser);
                        MessageBox.Show("Изменения сохранены.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error saving user: " + ex.Message);
                    }
                }
            }
        }

        private async void messages_grid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                var editedMessage = messages_grid.Rows[e.RowIndex].DataBoundItem as MessengerClient.Message;
                if (editedMessage != null)
                {
                    // Здесь вы можете сохранить изменения в объекте User
                    // Например, вызвать метод для обновления данных на сервере
                    try
                    {
                        await messengerclient.UpdateMessage(editedMessage);
                        MessageBox.Show("Изменения сохранены.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error saving user: " + ex.Message);
                    }
                }
            }
        }

        private void messages_Click(object sender, EventArgs e)
        {

        }
    }
}