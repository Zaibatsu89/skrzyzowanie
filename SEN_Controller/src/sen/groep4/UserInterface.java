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
import java.util.ArrayList;
import java.util.Calendar;
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
    private JTextArea _textArea;
    private JFileChooser _fileChooser;
    
    public UserInterface()
    {
	setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
	
	addComponentsToPane(getContentPane());
	
	pack();
	setSize(new Dimension(450, 900));
	setTitle("SEN Simulator");
	setLocationRelativeTo(null);
	setVisible(true);
    }
    
    private void addComponentsToPane(Container pane)
    {
	// Layout
	pane.setLayout(new BoxLayout(pane, BoxLayout.Y_AXIS));
	
	JPanel controlsLoad = new JPanel();
	controlsLoad.setLayout(new GridLayout(0,1));
	
        JPanel controlsConnect = new JPanel();
        controlsConnect.setLayout(new GridLayout(0,3));
	
        JPanel console = new JPanel();
        console.setLayout(new GridLayout(0,1));
	
	// Controls
	JButton btnLoad = new JButton("Load input file");
	
	controlsLoad.add(btnLoad);
	
	JLabel lblAddress = new JLabel("Simulator IP:");
	lblAddress.setHorizontalAlignment(SwingConstants.CENTER);
	JTextField txtAddress = new JTextField();
	JButton btnConnect = new JButton("Connect");
	btnConnect.setEnabled(false);
	
        controlsConnect.add(lblAddress);
	controlsConnect.add(txtAddress);
	controlsConnect.add(btnConnect);
	
	_textArea = new JTextArea(44, 0);
	_textArea.setEditable(false);
	
	JScrollPane scrollPane = new JScrollPane(_textArea); 
	scrollPane.setAutoscrolls(true);
	
	console.add(scrollPane);
        
	_fileChooser = new JFileChooser();
	_fileChooser.setAcceptAllFileFilterUsed(false);
	_fileChooser.setFileFilter(new XMLFileFilter());
	
        // Event
        btnLoad.addActionListener(new ActionListener()
	{
	    @Override
            public void actionPerformed(ActionEvent e)
	    {
		int returnVal = _fileChooser.showOpenDialog(new JPanel());
		
		if (returnVal == JFileChooser.APPROVE_OPTION)
		{
		    File file = _fileChooser.getSelectedFile();
		    
		    readXMLFile(file);
		}
		else
		{
		    setText("Open command cancelled by user.");
		}
            }
        });
	
	// Pane
	pane.add(controlsLoad, pane);
        pane.add(controlsConnect, pane);
        pane.add(console, pane);
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
	
	Main.xml = xmlParser.parse(listInput);
	
	showXMLFile();
    }
    
    private void setText(String text)
    {
	_textArea.append("[" + Calendar.getInstance().getTime().getTime() + "] " + text + "\n");
    }
    
    private void showXMLFile()
    {
	for (int i = 0; i < Main.xml.size(); i ++)
	{
	    setText(Main.xml.get(i));
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
		if (extension.equals("xml"))
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
	    return "Input files (*.xml)";
	}
	
	private String getExtension(File file)
	{
	    String ext = null;
	    String s = file.getName();
	    int i = s.lastIndexOf('.');

	    if (i > 0 &&  i < s.length() - 1)
	    {
		ext = s.substring(i+1).toLowerCase();
	    }

	    return ext;
	}
    }
}