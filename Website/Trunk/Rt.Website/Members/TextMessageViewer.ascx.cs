using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using MichaelBrumm.Win32;
using Rt.Framework.Components;
using Rt.Website;

public partial class Members_TextMessageViewer : UserControl
{
	private TextMessage _textMessage;

	public void SetTextMessage(TextMessage textMessage)
	{
		_textMessage = textMessage;
	}

	protected void Page_Load(object sender, EventArgs e)
	{
		if (_textMessage == null)
		{
			TextMessagePanel.Visible = false;
		}
		else
		{
			Win32TimeZone timeZone = TimeZones.GetTimeZone(Profile.TimeZoneIndex);

			TextMessagePanel.Visible = true;
			TextMessageTimestamp.Text = timeZone.ToLocalTime(_textMessage.Timestamp).ToString();
			TextMessage.Text = _textMessage.Message;
			Acknowledge.CommandArgument = _textMessage.Id.ToString();
		}
	}

	protected void Acknowledge_Click(object sender, EventArgs e)
	{
		var acknowledgeButton = (LinkButton) sender;
		TextMessagesInterface.AcknowledgeMessage(Global.GetDbConnection(), int.Parse(acknowledgeButton.CommandArgument));
		TextMessagePanel.Style.Add("display", "none");
	}
}