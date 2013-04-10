/**
 * 
 */
package rt.javaTrayApplication;

import java.awt.Dimension;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;

import javax.swing.JButton;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JOptionPane;
import javax.swing.JPanel;
import javax.swing.JPasswordField;
import javax.swing.JTextField;
import javax.swing.SpringLayout;

import org.apache.log4j.Logger;

import rt.javaTrayApplication.configuration.Settings;

/**
 * @author shawnriesterer
 *
 */
public class MainForm extends JFrame implements ActionListener {
	private static Logger _log = Logger.getLogger(MainForm.class);
	
	private String BLANK_PASSWORD = "asd;lkefaasdf";
	
	private boolean _forceEnteringOptions = false;
	
	private static final long serialVersionUID = 3267628807131250028L;
	private SpringLayout credentialsLayout;
	private JPanel credentialsPanel;
	private JLabel usernameLabel;
	private JTextField txtUsername;
	private JLabel passwordLabel;
	private JPasswordField txtPassword;
	private JButton ok;
	private JButton cancel;
	
	public MainForm()
	{
		InitializeComponents();
	}
	
	private Settings getSettings()
	{
		return MainApplicationContext.getSettings();
	}
	
	private void setSettings(Settings value)
	{
		MainApplicationContext.setSettings(value);
	}
	
	private void InitializeComponents()
	{
		credentialsLayout = new SpringLayout();
		credentialsPanel = new JPanel(credentialsLayout);
		usernameLabel = new JLabel("Username:");
		txtUsername = new JTextField("", 15);
		passwordLabel = new JLabel("Password:");
		txtPassword = new JPasswordField("", 15);
		ok = new JButton("OK");
		cancel = new JButton("Cancel");
		//
        // this
		//
		this.setName("RankTrend Tray Preferences");
		this.setTitle("Rank Trend Tray Preferences");
        this.getRootPane().setDefaultButton(ok);
        //
        // credentialsPanel
        //
        credentialsPanel.setOpaque(true);
        credentialsPanel.setPreferredSize(new Dimension(300, 100));
        this.getContentPane().add(credentialsPanel);
        //
        // usernameLabel
        //
        usernameLabel.setHorizontalAlignment(JLabel.RIGHT);
        usernameLabel.setPreferredSize(new Dimension(75, 20));
        credentialsPanel.add(usernameLabel);
        credentialsLayout.putConstraint(SpringLayout.WEST, usernameLabel,
        		5,
        		SpringLayout.WEST, credentialsPanel);
        credentialsLayout.putConstraint(SpringLayout.NORTH, usernameLabel,
        		5,
        		SpringLayout.NORTH, credentialsPanel);
        //
        // txtUsername
        //
        credentialsPanel.add(txtUsername);
        credentialsLayout.putConstraint(SpringLayout.WEST, txtUsername,
        		5,
        		SpringLayout.EAST, usernameLabel);
        credentialsLayout.putConstraint(SpringLayout.NORTH, txtUsername,
        		5,
        		SpringLayout.NORTH, credentialsPanel);
        //
        // passwordLabel
        //
        passwordLabel.setHorizontalAlignment(JLabel.RIGHT);
        passwordLabel.setPreferredSize(new Dimension(75, 20));
        credentialsPanel.add(passwordLabel);
        credentialsLayout.putConstraint(SpringLayout.WEST, passwordLabel,
        		5,
        		SpringLayout.WEST, credentialsPanel);
        credentialsLayout.putConstraint(SpringLayout.NORTH, passwordLabel,
        		5,
        		SpringLayout.SOUTH, usernameLabel);
        //
        // txtPassword
        //
        credentialsPanel.add(txtPassword);
        credentialsLayout.putConstraint(SpringLayout.WEST, txtPassword,
        		5,
        		SpringLayout.EAST, passwordLabel);
        credentialsLayout.putConstraint(SpringLayout.NORTH, txtPassword,
        		5,
        		SpringLayout.SOUTH, txtUsername);
        //
        // cancel
        //
        cancel.setActionCommand("cancel");
        cancel.addActionListener(this);
        credentialsPanel.add(cancel);
        credentialsLayout.putConstraint(SpringLayout.EAST, cancel,
        		-10,
        		SpringLayout.EAST, credentialsPanel);
        credentialsLayout.putConstraint(SpringLayout.SOUTH, cancel,
        		-10,
        		SpringLayout.SOUTH, credentialsPanel);
        //
        // ok
        //
        ok.setActionCommand("ok");
        ok.addActionListener(this);
        credentialsPanel.add(ok);
        credentialsLayout.putConstraint(SpringLayout.EAST, ok,
        		-5,
        		SpringLayout.WEST, cancel);
        credentialsLayout.putConstraint(SpringLayout.SOUTH, ok,
        		-10,
        		SpringLayout.SOUTH, credentialsPanel);
        
        this.pack();
	}
	
	public void Show()
	{
		if (getSettings() == null)
		{
			_log.info("Saved options could not be found, so a new set with default options is being created");
			setSettings(new Settings());
			_forceEnteringOptions = true;
		}
		populateForm();
		
		this.setVisible(true);
	}
	
	public void Hide()
	{
		this.setVisible(false);
	}

	public void actionPerformed(ActionEvent arg0) {
		Object source = arg0.getSource();
		if(source == ok)
		{
			ok_Click();
		}
		else if(source == cancel)
		{
			cancel_Click();
		}
	}
	
	private void populateForm()
	{
		Settings settings = getSettings();
		txtUsername.setText(settings.getUserName());

		//Load the default password so we can tell when it changes
		txtPassword.setText(BLANK_PASSWORD);
	}
	
	private void ok_Click()
	{		
		ok.setEnabled(false);

		if (!saveSettings())
		{
			ok.setEnabled(true);
			return;
		}
		else
			MainApplicationContext.ShowMessageBox("Your credentials have been successfully saved.");
		
		Hide();

		//Re-enabled the "Save" button
		ok.setEnabled(true);

		//OnUserCredentialsSave(new EventArgs());
	}
	
	private void cancel_Click()
	{
		int dr = JOptionPane.OK_OPTION;

		if(_forceEnteringOptions)
			dr = JOptionPane.showConfirmDialog(this, "You have not yet configured your options. This application will not work until you do.", MainApplicationContext.APP_NAME, JOptionPane.OK_CANCEL_OPTION);

		if (_forceEnteringOptions && dr == JOptionPane.OK_OPTION)
			System.exit(0);
		else if(!_forceEnteringOptions)
			Hide();
	}
	
	private boolean saveSettings()
	{
		Settings settings = getSettings();
		
		if (txtUsername.getText().length() == 0)
		{
			JOptionPane.showMessageDialog(this, "You have entered invalid credentials.");
			/*MessageBox.Show("You have entered invalid credentials",
				MainApplicationContext.APP_NAME + " - Invalid Credentials", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);*/

			return false;
		}

		if (settings.getUserName() != txtUsername.getText() || txtPassword.getPassword().toString() != BLANK_PASSWORD)
			if (!changeCredentials())
				return false;

		Settings.SaveSettings(settings);

		return true;
	}
	
	private boolean changeCredentials()
	{
		webservices.TrayApplication ws;
		//Guid guid;
		String guidString;

		if (txtPassword.getPassword().toString() == BLANK_PASSWORD)
		{
			MainApplicationContext.ShowMessageBox("If you change your user name, you need to re-enter your password. Your user name has not been changed.");
			return false;
		}

		//The username or password has changed, so we need to get a new GUID
		ws = new webservices.TrayApplication();

		//Cursor = Cursors.WaitCursor;
		try
		{
			guidString = ws.Authenticate(txtUsername.getText(), txtPassword.getPassword());
			//guidString = "SampleGuid";
		} catch (Exception e) {
			_log.warn("An error occurred while attempting to authenticate.  This may just be that it was unable to communicate with the server.", e);
			MainApplicationContext.ShowMessageBox("Unable to authenticate at this time.  Try again later or check the log for a more detailed reason.");
			return false;
		}
		finally
		{
			//Cursor = Cursors.Default;
		}

		if (guidString == null)
		{
			MainApplicationContext.ShowMessageBox("The user name and password you entered are not correct.");
			return false;
		}

		//guid = new Guid(guidString);
		Settings settings = getSettings();
		settings.setUserName(txtUsername.getText());
		settings.setUserGuid(guidString);

		return true;
	}
}
