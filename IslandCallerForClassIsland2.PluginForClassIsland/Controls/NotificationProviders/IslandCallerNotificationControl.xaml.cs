using System.Windows.Controls;
namespace IslandCallerForClassIsland2.Controls.NotificationProviders;

public partial class IslandCallerNotificationControl : UserControl
{
    public IslandCallerNotificationControl(string studentName)
    {
        InitializeComponent();
        RandomStudent.Text = studentName;
    }
}