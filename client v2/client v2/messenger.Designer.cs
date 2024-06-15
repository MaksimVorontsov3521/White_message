namespace client_v2
{
    partial class messenger
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            clientmenu = new ContextMenuStrip(components);
            ls = new ToolStripMenuItem();
            tabControl1 = new TabControl();
            groupchat = new TabPage();
            filebutton = new Button();
            yournick = new Label();
            IsYouOnline = new Label();
            reconnect = new Button();
            send = new Button();
            mess = new TextBox();
            online = new ListBox();
            label2 = new Label();
            chat = new TextBox();
            personmessages = new TabPage();
            contact = new Label();
            yournickpers = new Label();
            IsYouOnlinePers = new Label();
            personalsend = new Button();
            personalmess = new TextBox();
            personalchat = new TextBox();
            label1 = new Label();
            yourcontacts = new ListBox();
            clientmenu.SuspendLayout();
            tabControl1.SuspendLayout();
            groupchat.SuspendLayout();
            personmessages.SuspendLayout();
            SuspendLayout();
            // 
            // clientmenu
            // 
            clientmenu.Items.AddRange(new ToolStripItem[] { ls });
            clientmenu.Name = "clientmenu";
            clientmenu.Size = new Size(238, 26);
            clientmenu.Text = "Выберите действие";
            // 
            // ls
            // 
            ls.Name = "ls";
            ls.Size = new Size(237, 22);
            ls.Text = "Написать личное сообщение";
            ls.Click += ls_Click;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(groupchat);
            tabControl1.Controls.Add(personmessages);
            tabControl1.Location = new Point(0, -1);
            tabControl1.Margin = new Padding(4, 3, 4, 3);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(966, 540);
            tabControl1.TabIndex = 1;
            tabControl1.SelectedIndexChanged += tabControl1_SelectedIndexChanged;
            // 
            // groupchat
            // 
            groupchat.Controls.Add(filebutton);
            groupchat.Controls.Add(yournick);
            groupchat.Controls.Add(IsYouOnline);
            groupchat.Controls.Add(reconnect);
            groupchat.Controls.Add(send);
            groupchat.Controls.Add(mess);
            groupchat.Controls.Add(online);
            groupchat.Controls.Add(label2);
            groupchat.Controls.Add(chat);
            groupchat.Location = new Point(4, 24);
            groupchat.Margin = new Padding(4, 3, 4, 3);
            groupchat.Name = "groupchat";
            groupchat.Padding = new Padding(4, 3, 4, 3);
            groupchat.Size = new Size(958, 512);
            groupchat.TabIndex = 0;
            groupchat.Text = "Групповой чат";
            groupchat.UseVisualStyleBackColor = true;
            // 
            // filebutton
            // 
            filebutton.Location = new Point(851, 450);
            filebutton.Name = "filebutton";
            filebutton.Size = new Size(100, 43);
            filebutton.TabIndex = 21;
            filebutton.Text = "Отправить файл";
            filebutton.UseVisualStyleBackColor = true;
            filebutton.Click += filebutton_Click;
            // 
            // yournick
            // 
            yournick.AutoSize = true;
            yournick.Location = new Point(750, 21);
            yournick.Margin = new Padding(4, 0, 4, 0);
            yournick.Name = "yournick";
            yournick.Size = new Size(0, 15);
            yournick.TabIndex = 20;
            yournick.TextChanged += yournick_TextChanged;
            // 
            // IsYouOnline
            // 
            IsYouOnline.AutoSize = true;
            IsYouOnline.Location = new Point(750, 0);
            IsYouOnline.Margin = new Padding(4, 0, 4, 0);
            IsYouOnline.Name = "IsYouOnline";
            IsYouOnline.Size = new Size(75, 15);
            IsYouOnline.TabIndex = 19;
            IsYouOnline.Text = "Вы не в сети";
            IsYouOnline.TextChanged += IsYouOnline_TextChanged;
            // 
            // reconnect
            // 
            reconnect.Location = new Point(7, 450);
            reconnect.Margin = new Padding(4, 3, 4, 3);
            reconnect.Name = "reconnect";
            reconnect.Size = new Size(102, 44);
            reconnect.TabIndex = 18;
            reconnect.Text = "Войти в другой аккаунт";
            reconnect.UseVisualStyleBackColor = true;
            reconnect.Click += reconnect_Click;
            // 
            // send
            // 
            send.Location = new Point(750, 450);
            send.Margin = new Padding(4, 3, 4, 3);
            send.Name = "send";
            send.Size = new Size(94, 44);
            send.TabIndex = 17;
            send.Text = "Отправить";
            send.UseVisualStyleBackColor = true;
            send.Click += send_Click;
            // 
            // mess
            // 
            mess.Location = new Point(359, 450);
            mess.Margin = new Padding(4, 3, 4, 3);
            mess.Multiline = true;
            mess.Name = "mess";
            mess.Size = new Size(383, 43);
            mess.TabIndex = 16;
            // 
            // online
            // 
            online.ContextMenuStrip = clientmenu;
            online.FormattingEnabled = true;
            online.ItemHeight = 15;
            online.Location = new Point(9, 22);
            online.Margin = new Padding(4, 3, 4, 3);
            online.Name = "online";
            online.Size = new Size(342, 409);
            online.TabIndex = 15;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(7, 3);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(86, 15);
            label2.TabIndex = 14;
            label2.Text = "Сейчас в сети:";
            // 
            // chat
            // 
            chat.Location = new Point(359, 21);
            chat.Margin = new Padding(4, 3, 4, 3);
            chat.Multiline = true;
            chat.Name = "chat";
            chat.ReadOnly = true;
            chat.Size = new Size(383, 409);
            chat.TabIndex = 13;
            // 
            // personmessages
            // 
            personmessages.Controls.Add(contact);
            personmessages.Controls.Add(yournickpers);
            personmessages.Controls.Add(IsYouOnlinePers);
            personmessages.Controls.Add(personalsend);
            personmessages.Controls.Add(personalmess);
            personmessages.Controls.Add(personalchat);
            personmessages.Controls.Add(label1);
            personmessages.Controls.Add(yourcontacts);
            personmessages.Location = new Point(4, 24);
            personmessages.Margin = new Padding(4, 3, 4, 3);
            personmessages.Name = "personmessages";
            personmessages.Padding = new Padding(4, 3, 4, 3);
            personmessages.Size = new Size(958, 512);
            personmessages.TabIndex = 1;
            personmessages.Text = "Личные сообщения";
            personmessages.UseVisualStyleBackColor = true;
            // 
            // contact
            // 
            contact.AutoSize = true;
            contact.Location = new Point(453, 4);
            contact.Name = "contact";
            contact.Size = new Size(0, 15);
            contact.TabIndex = 23;
            // 
            // yournickpers
            // 
            yournickpers.AutoSize = true;
            yournickpers.Location = new Point(772, 20);
            yournickpers.Margin = new Padding(4, 0, 4, 0);
            yournickpers.Name = "yournickpers";
            yournickpers.Size = new Size(0, 15);
            yournickpers.TabIndex = 22;
            // 
            // IsYouOnlinePers
            // 
            IsYouOnlinePers.AutoSize = true;
            IsYouOnlinePers.Location = new Point(770, 7);
            IsYouOnlinePers.Margin = new Padding(4, 0, 4, 0);
            IsYouOnlinePers.Name = "IsYouOnlinePers";
            IsYouOnlinePers.Size = new Size(75, 15);
            IsYouOnlinePers.TabIndex = 21;
            IsYouOnlinePers.Text = "Вы не в сети";
            // 
            // personalsend
            // 
            personalsend.Location = new Point(670, 437);
            personalsend.Margin = new Padding(4, 3, 4, 3);
            personalsend.Name = "personalsend";
            personalsend.Size = new Size(94, 44);
            personalsend.TabIndex = 20;
            personalsend.Text = "Отправить";
            personalsend.UseVisualStyleBackColor = true;
            personalsend.Click += personalsend_Click;
            // 
            // personalmess
            // 
            personalmess.Location = new Point(361, 437);
            personalmess.Margin = new Padding(4, 3, 4, 3);
            personalmess.Multiline = true;
            personalmess.Name = "personalmess";
            personalmess.Size = new Size(301, 43);
            personalmess.TabIndex = 19;
            // 
            // personalchat
            // 
            personalchat.Location = new Point(361, 22);
            personalchat.Margin = new Padding(4, 3, 4, 3);
            personalchat.Multiline = true;
            personalchat.Name = "personalchat";
            personalchat.ReadOnly = true;
            personalchat.Size = new Size(401, 409);
            personalchat.TabIndex = 18;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(9, 3);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(95, 15);
            label1.TabIndex = 17;
            label1.Text = "Ваши контакты:";
            // 
            // yourcontacts
            // 
            yourcontacts.ContextMenuStrip = clientmenu;
            yourcontacts.FormattingEnabled = true;
            yourcontacts.ItemHeight = 15;
            yourcontacts.Location = new Point(9, 22);
            yourcontacts.Margin = new Padding(4, 3, 4, 3);
            yourcontacts.Name = "yourcontacts";
            yourcontacts.Size = new Size(344, 409);
            yourcontacts.TabIndex = 16;
            yourcontacts.SelectedIndexChanged += yourcontacts_SelectedIndexChanged;
            // 
            // messenger
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(963, 551);
            Controls.Add(tabControl1);
            Margin = new Padding(4, 3, 4, 3);
            Name = "messenger";
            Text = " Ты кто";
            clientmenu.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            groupchat.ResumeLayout(false);
            groupchat.PerformLayout();
            personmessages.ResumeLayout(false);
            personmessages.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip clientmenu;
        private System.Windows.Forms.ToolStripMenuItem ls;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage groupchat;
        private System.Windows.Forms.TabPage personmessages;
        private System.Windows.Forms.Label yournick;
        private System.Windows.Forms.Label IsYouOnline;
        private System.Windows.Forms.Button reconnect;
        private System.Windows.Forms.Button send;
        private System.Windows.Forms.TextBox mess;
        private System.Windows.Forms.ListBox online;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox chat;
        private System.Windows.Forms.TextBox personalchat;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox yourcontacts;
        private System.Windows.Forms.Button personalsend;
        private System.Windows.Forms.TextBox personalmess;
        private Label yournickpers;
        private Label IsYouOnlinePers;
        private Label contact;
        private Button filebutton;
    }
}

