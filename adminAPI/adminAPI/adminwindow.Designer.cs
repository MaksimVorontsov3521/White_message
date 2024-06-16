namespace adminAPI
{
    partial class adminwindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tabControl1 = new TabControl();
            account = new TabPage();
            users_grid = new DataGridView();
            reload_users = new Button();
            addcontact = new Button();
            contactId = new TextBox();
            myId = new TextBox();
            label10 = new Label();
            label9 = new Label();
            label8 = new Label();
            reset_autoincrement_users = new Button();
            delete_all_users = new Button();
            delete_acc_by_id = new Button();
            accid = new TextBox();
            label7 = new Label();
            label6 = new Label();
            add = new Button();
            post = new TextBox();
            fio = new TextBox();
            password = new TextBox();
            login = new TextBox();
            label5 = new Label();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            messages = new TabPage();
            reload_messages = new Button();
            messages_grid = new DataGridView();
            delete_all_messages = new Button();
            reset_autoincrement_messages = new Button();
            tabControl1.SuspendLayout();
            account.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)users_grid).BeginInit();
            messages.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)messages_grid).BeginInit();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(account);
            tabControl1.Controls.Add(messages);
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1274, 514);
            tabControl1.TabIndex = 0;
            // 
            // account
            // 
            account.Controls.Add(users_grid);
            account.Controls.Add(reload_users);
            account.Controls.Add(addcontact);
            account.Controls.Add(contactId);
            account.Controls.Add(myId);
            account.Controls.Add(label10);
            account.Controls.Add(label9);
            account.Controls.Add(label8);
            account.Controls.Add(reset_autoincrement_users);
            account.Controls.Add(delete_all_users);
            account.Controls.Add(delete_acc_by_id);
            account.Controls.Add(accid);
            account.Controls.Add(label7);
            account.Controls.Add(label6);
            account.Controls.Add(add);
            account.Controls.Add(post);
            account.Controls.Add(fio);
            account.Controls.Add(password);
            account.Controls.Add(login);
            account.Controls.Add(label5);
            account.Controls.Add(label4);
            account.Controls.Add(label3);
            account.Controls.Add(label2);
            account.Controls.Add(label1);
            account.Location = new Point(4, 24);
            account.Name = "account";
            account.Padding = new Padding(3);
            account.Size = new Size(1266, 486);
            account.TabIndex = 0;
            account.Text = "account";
            account.UseVisualStyleBackColor = true;
            // 
            // users_grid
            // 
            users_grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            users_grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            users_grid.Location = new Point(3, 245);
            users_grid.Name = "users_grid";
            users_grid.Size = new Size(1253, 174);
            users_grid.TabIndex = 25;
            users_grid.CellEndEdit += users_grid_CellEndEdit;
            users_grid.SelectionChanged += users_grid_SelectionChanged;
            // 
            // reload_users
            // 
            reload_users.Location = new Point(1054, 437);
            reload_users.Name = "reload_users";
            reload_users.Size = new Size(202, 48);
            reload_users.TabIndex = 24;
            reload_users.Text = "Обновить";
            reload_users.UseVisualStyleBackColor = true;
            reload_users.Click += reload_grid_Click;
            // 
            // addcontact
            // 
            addcontact.Location = new Point(381, 173);
            addcontact.Name = "addcontact";
            addcontact.Size = new Size(121, 23);
            addcontact.TabIndex = 22;
            addcontact.Text = "Добавить контакт";
            addcontact.UseVisualStyleBackColor = true;
            addcontact.Click += addcontact_Click;
            // 
            // contactId
            // 
            contactId.Location = new Point(292, 173);
            contactId.Name = "contactId";
            contactId.Size = new Size(68, 23);
            contactId.TabIndex = 21;
            // 
            // myId
            // 
            myId.Location = new Point(67, 173);
            myId.Name = "myId";
            myId.Size = new Size(127, 23);
            myId.TabIndex = 20;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(292, 155);
            label10.Name = "label10";
            label10.Size = new Size(68, 15);
            label10.TabIndex = 19;
            label10.Text = "id контакта";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(3, 155);
            label9.Name = "label9";
            label9.Size = new Size(268, 15);
            label9.TabIndex = 18;
            label9.Text = "id пользователя которому добавляется контакт";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(3, 130);
            label8.Name = "label8";
            label8.Size = new Size(104, 15);
            label8.TabIndex = 17;
            label8.Text = "Добавить контакт";
            // 
            // reset_autoincrement_users
            // 
            reset_autoincrement_users.Location = new Point(508, 105);
            reset_autoincrement_users.Name = "reset_autoincrement_users";
            reset_autoincrement_users.Size = new Size(160, 23);
            reset_autoincrement_users.TabIndex = 16;
            reset_autoincrement_users.Text = "Сбросить автоинкремент";
            reset_autoincrement_users.UseVisualStyleBackColor = true;
            reset_autoincrement_users.Click += reset_autoincrement_users_Click;
            // 
            // delete_all_users
            // 
            delete_all_users.Location = new Point(327, 105);
            delete_all_users.Name = "delete_all_users";
            delete_all_users.Size = new Size(175, 23);
            delete_all_users.TabIndex = 15;
            delete_all_users.Text = "Удалить всех пользователей";
            delete_all_users.UseVisualStyleBackColor = true;
            delete_all_users.Click += delete_all_users_Click;
            // 
            // delete_acc_by_id
            // 
            delete_acc_by_id.Location = new Point(178, 104);
            delete_acc_by_id.Name = "delete_acc_by_id";
            delete_acc_by_id.Size = new Size(143, 24);
            delete_acc_by_id.TabIndex = 13;
            delete_acc_by_id.Text = "Удалить пользователя";
            delete_acc_by_id.UseVisualStyleBackColor = true;
            delete_acc_by_id.Click += delete_acc_by_id_Click;
            // 
            // accid
            // 
            accid.Location = new Point(32, 104);
            accid.Name = "accid";
            accid.Size = new Size(140, 23);
            accid.TabIndex = 12;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(6, 104);
            label7.Name = "label7";
            label7.Size = new Size(20, 15);
            label7.TabIndex = 11;
            label7.Text = "id ";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(3, 76);
            label6.Name = "label6";
            label6.Size = new Size(126, 15);
            label6.TabIndex = 10;
            label6.Text = "Удалить аккаунт по id";
            // 
            // add
            // 
            add.Location = new Point(439, 48);
            add.Name = "add";
            add.Size = new Size(147, 24);
            add.TabIndex = 9;
            add.Text = "Добавить пользователя";
            add.UseVisualStyleBackColor = true;
            add.Click += add_Click;
            // 
            // post
            // 
            post.Location = new Point(334, 50);
            post.Name = "post";
            post.Size = new Size(99, 23);
            post.TabIndex = 8;
            // 
            // fio
            // 
            fio.Location = new Point(226, 50);
            fio.Name = "fio";
            fio.Size = new Size(102, 23);
            fio.TabIndex = 7;
            // 
            // password
            // 
            password.Location = new Point(113, 50);
            password.Name = "password";
            password.Size = new Size(107, 23);
            password.TabIndex = 6;
            // 
            // login
            // 
            login.Location = new Point(6, 50);
            login.Name = "login";
            login.Size = new Size(101, 23);
            login.TabIndex = 5;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(334, 32);
            label5.Name = "label5";
            label5.Size = new Size(30, 15);
            label5.TabIndex = 4;
            label5.Text = "post";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(226, 32);
            label4.Name = "label4";
            label4.Size = new Size(21, 15);
            label4.TabIndex = 3;
            label4.Text = "fio";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(113, 32);
            label3.Name = "label3";
            label3.Size = new Size(57, 15);
            label3.TabIndex = 2;
            label3.Text = "password";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(3, 32);
            label2.Name = "label2";
            label2.Size = new Size(34, 15);
            label2.TabIndex = 1;
            label2.Text = "login";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(3, 3);
            label1.Name = "label1";
            label1.Size = new Size(104, 15);
            label1.TabIndex = 0;
            label1.Text = "Добавить аккаунт";
            // 
            // messages
            // 
            messages.Controls.Add(reload_messages);
            messages.Controls.Add(messages_grid);
            messages.Controls.Add(delete_all_messages);
            messages.Controls.Add(reset_autoincrement_messages);
            messages.Location = new Point(4, 24);
            messages.Name = "messages";
            messages.Padding = new Padding(3);
            messages.Size = new Size(1266, 486);
            messages.TabIndex = 1;
            messages.Text = "messages";
            messages.UseVisualStyleBackColor = true;
            // 
            // reload_messages
            // 
            reload_messages.Location = new Point(907, 432);
            reload_messages.Name = "reload_messages";
            reload_messages.Size = new Size(156, 46);
            reload_messages.TabIndex = 4;
            reload_messages.Text = "обновить";
            reload_messages.UseVisualStyleBackColor = true;
            reload_messages.Click += reload_messages_Click;
            // 
            // messages_grid
            // 
            messages_grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            messages_grid.Location = new Point(3, 39);
            messages_grid.Name = "messages_grid";
            messages_grid.Size = new Size(1060, 387);
            messages_grid.TabIndex = 3;
            messages_grid.CellEndEdit += messages_grid_CellEndEdit;
            // 
            // delete_all_messages
            // 
            delete_all_messages.Location = new Point(202, 6);
            delete_all_messages.Name = "delete_all_messages";
            delete_all_messages.Size = new Size(178, 22);
            delete_all_messages.TabIndex = 2;
            delete_all_messages.Text = "Удалить все сообщения";
            delete_all_messages.UseVisualStyleBackColor = true;
            delete_all_messages.Click += delete_all_messages_Click;
            // 
            // reset_autoincrement_messages
            // 
            reset_autoincrement_messages.Location = new Point(6, 6);
            reset_autoincrement_messages.Name = "reset_autoincrement_messages";
            reset_autoincrement_messages.Size = new Size(178, 22);
            reset_autoincrement_messages.TabIndex = 1;
            reset_autoincrement_messages.Text = "Сбросить автоинкеремент";
            reset_autoincrement_messages.UseVisualStyleBackColor = true;
            reset_autoincrement_messages.Click += reset_autoincrement_messages_Click;
            // 
            // adminwindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1272, 514);
            Controls.Add(tabControl1);
            Name = "adminwindow";
            Text = "admin";
            tabControl1.ResumeLayout(false);
            account.ResumeLayout(false);
            account.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)users_grid).EndInit();
            messages.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)messages_grid).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl1;
        private TabPage account;
        private Button add;
        private TextBox post;
        private TextBox fio;
        private TextBox password;
        private TextBox login;
        private Label label5;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label1;
        private TabPage messages;
        private Label label7;
        private Label label6;
        private Button delete_acc_by_id;
        private TextBox accid;
        private Button delete_all_users;
        private Button reset_autoincrement_messages;
        private Button delete_all_messages;
        private Button reset_autoincrement_users;
        private Label label9;
        private Label label8;
        private Button addcontact;
        private TextBox contactId;
        private TextBox myId;
        private Label label10;
        private Button reload_users;
        private Button reload_messages;
        private DataGridView messages_grid;
        private DataGridView users_grid;
    }
}
