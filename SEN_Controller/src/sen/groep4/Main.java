package sen.groep4;

import java.net.InetAddress;
import java.util.List;
import javax.swing.SwingUtilities;
import javax.swing.UIManager;
import javax.swing.UnsupportedLookAndFeelException;

public class Main
{
    public static InetAddress ip;
    public static List<String> xml;
    
    public static void main(String[] args)
    {
        try
	{
            UIManager.setLookAndFeel("com.sun.java.swing.plaf.windows.WindowsLookAndFeel");
        }
	catch (UnsupportedLookAndFeelException | IllegalAccessException | InstantiationException | ClassNotFoundException ex){}
	
	SwingUtilities.invokeLater(new Runnable()
	{
	    @Override
	    public void run()
	    {
		UserInterface ui = new UserInterface();
	    }
	});
    }
}