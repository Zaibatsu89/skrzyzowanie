package sen.groep4;

import java.io.IOException;
import javax.swing.SwingUtilities;
import javax.swing.UIManager;
import javax.swing.UnsupportedLookAndFeelException;

public class Main
{
    public static void main(String[] args)
    {
        try
	{
            UIManager.setLookAndFeel(Strings.ui);
        }
	catch (UnsupportedLookAndFeelException | IllegalAccessException | InstantiationException | ClassNotFoundException ex){}
	
	SwingUtilities.invokeLater(new Runnable()
	{
	    @Override
	    public void run()
	    {
		try
		{
		    new UserInterface();
		}
		catch (IOException ex){UserInterface.setText(ex.getMessage());}
	    }
	});
    }
}