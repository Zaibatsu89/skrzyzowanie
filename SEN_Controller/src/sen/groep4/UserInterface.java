package sen.groep4;

import java.awt.Container;
import java.awt.Dimension;
import java.awt.GridLayout;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.io.BufferedReader;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.io.IOException;
import java.net.InetAddress;
import java.net.NetworkInterface;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.Enumeration;
import java.util.List;
import javax.swing.BoxLayout;
import javax.swing.JButton;
import javax.swing.JFileChooser;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.swing.JScrollPane;
import javax.swing.JTextArea;
import javax.swing.JTextField;
import javax.swing.SwingConstants;
import javax.swing.filechooser.FileFilter;

public class UserInterface extends JFrame
{
    private JButton _btnLoad;
    private JButton _btnStart;
    private Communication _communication;
    private JFileChooser _fileChooser;
    private List<String> _xml;
    private static JTextArea _txtConsole;  
    private JTextField _txtPort;
    
    public UserInterface() throws IOException
    {
	setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
	
	addComponentsToPane(getContentPane());
	
	pack();
	setSize(new Dimension(333, 777));
	setTitle(Strings.title + Strings.space + Strings.four);
	setLocationRelativeTo(null);
	setVisible(true);
    }
    
    public static void setText(String text)
    {
	Date date = Calendar.getInstance().getTime();
	String time = new SimpleDateFormat("HH:mm:ss.SSS").format(date);
	
	_txtConsole.append(
	    time +
	    Strings.space +
	    text +
	    Strings.newLine
	);
    }
    
    private void addComponentsToPane(Container pane) throws IOException
    {
	// Layout
	pane.setLayout(new BoxLayout(pane, BoxLayout.Y_AXIS));
	
	JPanel labels = new JPanel();
	labels.setLayout(new GridLayout(0,2));
	
	JPanel port = new JPanel();
	port.setLayout(new GridLayout(0,2));
	
        JPanel controls = new JPanel();
        controls.setLayout(new GridLayout(0,2));
	
        JPanel console = new JPanel();
        console.setLayout(new GridLayout(0,1));
	
	// Controls
	JLabel lblAddress = new JLabel(
	    Strings.space +
	    Strings.addressKey +
	    Strings.space +
	    getIPAddress()
	);
	
	labels.add(lblAddress);
	
	JLabel lblPort = new JLabel(Strings.portKey + Strings.space);
	lblPort.setHorizontalAlignment(SwingConstants.RIGHT);
	_txtPort = new JTextField();
	_txtPort.setText(
	    Strings.one +
	    Strings.three +
	    Strings.three +
	    Strings.seven
	);
	
	port.add(lblPort);
	port.add(_txtPort);
	
	labels.add(port);
	
	_btnLoad = new JButton(Strings.load);
	_btnStart = new JButton(Strings.start);
	_btnStart.setEnabled(false);
	
	controls.add(_btnLoad);
	controls.add(_btnStart);
	
	_txtConsole = new JTextArea(37, 0);
	_txtConsole.setEditable(false);
	
	JScrollPane scrollPane = new JScrollPane(_txtConsole); 
	scrollPane.setAutoscrolls(true);
	
	console.add(scrollPane);
        
	_fileChooser = new JFileChooser();
	_fileChooser.setAcceptAllFileFilterUsed(false);
	_fileChooser.setFileFilter(new XMLFileFilter());
	
        // Events
        _btnLoad.addActionListener(new ActionListener()
	{
	    @Override
            public void actionPerformed(ActionEvent e)
	    {
		int returnVal = _fileChooser.showOpenDialog(new JPanel());
		
		if (returnVal == JFileChooser.APPROVE_OPTION)
		{
		    File file = _fileChooser.getSelectedFile();
		    
		    readXMLFile(file);
		    
		    // Disable button Load
		    _btnLoad.setEnabled(false);
		    
		    // Enable button Connect
		    _btnStart.setEnabled(true);
		}
            }
        });
	
	_btnStart.addActionListener(new ActionListener()
	{
	    @Override
            public void actionPerformed(ActionEvent e)
	    {
		if (_btnStart.getText().equals(Strings.start))
		{
		    _communication = new Communication();
		    int port = Integer.valueOf(_txtPort.getText());
		    
		    try
		    {
			_communication.connect(port);
		    }
		    catch (IOException ex){setText(ex.getMessage());}
		    
		    // Disable button Load
		    _btnLoad.setEnabled(false);
		    
		    // Change text of button Start
		    _btnStart.setText(Strings.stop);
		}
		else
		{
		    _communication = null;
		    
		    // Enable button Load
		    _btnLoad.setEnabled(true);
		    
		    // Change text of button Start
		    _btnStart.setText(Strings.start);
		}
            }
        });
	
	// Pane
	pane.add(labels, pane);
        pane.add(controls, pane);
        pane.add(console, pane);
    }
    
    private String getIPAddress() throws IOException
    {
	String ipAddress =
	    Strings.one +
	    Strings.two +
	    Strings.seven +
	    Strings.dot +
	    Strings.zero +
	    Strings.dot +
	    Strings.zero +
	    Strings.dot +
	    Strings.one;
	List<String> addresses = new ArrayList();
	
	Enumeration e = NetworkInterface.getNetworkInterfaces();
        
	while (e.hasMoreElements())
        {
	    NetworkInterface n = (NetworkInterface) e.nextElement();
	    Enumeration ee = n.getInetAddresses();
	    
	    while (ee.hasMoreElements())
	    {
		InetAddress i = (InetAddress) ee.nextElement();
		
		addresses.add(i.getHostAddress());
	    }
	}
	
	for (int i = 0; i < addresses.size(); i ++)
	{
	    String address = addresses.get(i);
	    
	    if (address.length() <= 28 &&
		!address.startsWith(Strings.zero + Strings.doubleDot) &&
		!address.startsWith(Strings.one + Strings.zero) &&
		!address.startsWith(
		    Strings.one +
		    Strings.two +
		    Strings.seven
		) && !address.startsWith(
		    Strings.one +
		    Strings.seven +
		    Strings.two +
		    Strings.dot +
		    Strings.one +
		    Strings.six
		) && !address.startsWith(
		    Strings.one +
		    Strings.seven +
		    Strings.two +
		    Strings.dot +
		    Strings.one +
		    Strings.seven
		) && !address.startsWith(
		    Strings.one +
		    Strings.seven +
		    Strings.two +
		    Strings.dot +
		    Strings.one +
		    Strings.eight
		) && !address.startsWith(
		    Strings.one +
		    Strings.seven +
		    Strings.two +
		    Strings.dot +
		    Strings.one +
		    Strings.nine
		) && !address.startsWith(
		    Strings.one +
		    Strings.seven +
		    Strings.two +
		    Strings.dot +
		    Strings.two +
		    Strings.zero
		) && !address.startsWith(
		    Strings.one +
		    Strings.seven +
		    Strings.two +
		    Strings.dot +
		    Strings.two +
		    Strings.one
		) && !address.startsWith(
		    Strings.one +
		    Strings.seven +
		    Strings.two +
		    Strings.dot +
		    Strings.two +
		    Strings.two
		) && !address.startsWith(
		    Strings.one +
		    Strings.seven +
		    Strings.two +
		    Strings.dot +
		    Strings.two +
		    Strings.three
		) && !address.startsWith(
		    Strings.one +
		    Strings.seven +
		    Strings.two +
		    Strings.dot +
		    Strings.two +
		    Strings.four
		) && !address.startsWith(
		    Strings.one +
		    Strings.seven +
		    Strings.two +
		    Strings.dot +
		    Strings.two +
		    Strings.five
		) && !address.startsWith(
		    Strings.one +
		    Strings.seven +
		    Strings.two +
		    Strings.dot +
		    Strings.two +
		    Strings.six
		) && !address.startsWith(
		    Strings.one +
		    Strings.seven +
		    Strings.two +
		    Strings.dot +
		    Strings.two +
		    Strings.seven
		) && !address.startsWith(
		    Strings.one +
		    Strings.seven +
		    Strings.two +
		    Strings.dot +
		    Strings.two +
		    Strings.eight
		) && !address.startsWith(
		    Strings.one +
		    Strings.seven +
		    Strings.two +
		    Strings.dot +
		    Strings.two +
		    Strings.nine
		) && !address.startsWith(
		    Strings.one +
		    Strings.seven +
		    Strings.two +
		    Strings.dot +
		    Strings.three +
		    Strings.zero
		) && !address.startsWith(
		    Strings.one +
		    Strings.seven +
		    Strings.two +
		    Strings.dot +
		    Strings.three +
		    Strings.one
		) && !address.startsWith(
		    Strings.one +
		    Strings.nine +
		    Strings.two +
		    Strings.dot +
		    Strings.one +
		    Strings.six +
		    Strings.eight
		) && !address.startsWith(
		    Strings.two +
		    Strings.two +
		    Strings.four
		) && !address.startsWith(
		    Strings.two +
		    Strings.two +
		    Strings.five
		) && !address.startsWith(
		    Strings.two +
		    Strings.two +
		    Strings.six
		) && !address.startsWith(
		    Strings.two +
		    Strings.two +
		    Strings.seven
		) && !address.startsWith(
		    Strings.two +
		    Strings.two +
		    Strings.eight
		) && !address.startsWith(
		    Strings.two +
		    Strings.two +
		    Strings.nine
		) && !address.startsWith(
		    Strings.two +
		    Strings.three +
		    Strings.zero
		) && !address.startsWith(
		    Strings.two +
		    Strings.three +
		    Strings.one
		) && !address.startsWith(
		    Strings.two +
		    Strings.three +
		    Strings.two
		) && !address.startsWith(
		    Strings.two +
		    Strings.three +
		    Strings.four
		) && !address.startsWith(
		    Strings.two +
		    Strings.three +
		    Strings.five
		) && !address.startsWith(
		    Strings.two +
		    Strings.three +
		    Strings.six
		) && !address.startsWith(
		    Strings.two +
		    Strings.three +
		    Strings.seven
		) && !address.startsWith(
		    Strings.two +
		    Strings.three +
		    Strings.eight
		) && !address.startsWith(
		    Strings.two +
		    Strings.three +
		    Strings.nine
		) && !address.startsWith(
		    Strings.two +
		    Strings.five +
		    Strings.five +
		    Strings.dot +
		    Strings.two +
		    Strings.five +
		    Strings.five +
		    Strings.dot +
		    Strings.two +
		    Strings.five +
		    Strings.five +
		    Strings.dot +
		    Strings.two +
		    Strings.five +
		    Strings.five
		)
	    ){
		ipAddress = address;
	    }
	}
	
	return ipAddress;
    }
    
    private void readXMLFile(File file)
    {
	// Een invoerlijst wordt gemaakt
	List<String> listInput = new ArrayList();

	// Een bestandslezer wordt gemaakt
	FileReader fileReader = null;
	try
	{
	    fileReader = new FileReader(file);
	}
	catch (FileNotFoundException ex){}

	// Een bufferende lezer wordt gemaakt
	try (BufferedReader bufferedReader = new BufferedReader(fileReader))
	{
	    // Een invoerregel wordt gemaakt
	    String strInput;

	    // Zolang de invoerregel niet niets is
	    while ((strInput = bufferedReader.readLine()) != null)
	    {
		// De invoerregel wordt aan de invoerlijst toegevoegd
		listInput.add(strInput);
	    }
	}
	catch (IOException ex){}
	
	XMLParser xmlParser = new XMLParser();
	
	_xml = xmlParser.parse(listInput);
	
	showXMLFile();
    }
    
    private void showXMLFile()
    {
	for (int i = 0; i < _xml.size(); i ++)
	{
	    setText(_xml.get(i));
	}
    }
    
    private class XMLFileFilter extends FileFilter
    {
	@Override
	public boolean accept(File file)
	{
	    if (file.isDirectory())
	    {
		return true;
	    }
	    
	    String extension = getExtension(file);
	    
	    if (extension != null)
	    {
		if (extension.equals(Strings.xml))
		{
		    return true;
		}
		else
		{
		    return false;
		}
	    }
	    
	    return false;
	}
	
	@Override
	public String getDescription()
	{
	    return Strings.inputFiles;
	}
	
	private String getExtension(File file)
	{
	    String ext = null;
	    String s = file.getName();
	    int i = s.lastIndexOf('.');
	    
	    if (i > 0 &&  i < s.length() - 1)
	    {
		ext = s.substring(i + 1).toLowerCase();
	    }
	    
	    return ext;
	}
    }
}