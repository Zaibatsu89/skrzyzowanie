package sen.groep4;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.ServerSocket;
import java.net.Socket;

public class Communication
{
    public void connect(int port) throws IOException
    {
	boolean listening = true;
	
	ServerSocket serverSocket = new ServerSocket(port);
	
	UserInterface.setText(Strings.serverStarted);
	
	while (listening)
	{
	    new CommunicationThread(serverSocket.accept()).start();
	}
	
	serverSocket.close();
    }
    
    private class CommunicationThread extends Thread
    {
	private Socket socket = null;

	public CommunicationThread(Socket socket)
	{
	    super(Strings.communicationThread);
	    this.socket = socket;
	}
	
	@Override
	public void run()
	{
	    try
	    {
		BufferedReader in = new BufferedReader
		(
		    new InputStreamReader(socket.getInputStream())
		);
		PrintWriter out = new PrintWriter(socket.getOutputStream(), true);
		
		String inputLine, outputLine;
		Protocol protocol = new Protocol();
		outputLine = protocol.processInput(null);
		UserInterface.setText(String.format(Strings.sent, outputLine));
		out.println(outputLine);
		
		while ((inputLine = in.readLine()) != null)
		{
		    if (!inputLine.isEmpty())
		    {
			UserInterface.setText(
			    Strings.received +
			    Strings.space +
			    inputLine
			);
		    }
		    
		    outputLine = protocol.processInput(inputLine);
		    UserInterface.setText(
			Strings.sent +
			Strings.space +
			outputLine
		    );
		    out.println(outputLine);
		    
		    if (outputLine.startsWith(Strings.bye))
			break;
		}
		
		out.close();
		in.close();
		socket.close();
	    }
	    catch (IOException ex){UserInterface.setText(ex.getMessage());}
	}
    }
    
    private class Protocol
    {
	private static final int WAITING = 0;
	private static final int SENTKNOCKKNOCK = 1;
	private static final int SENTCLUE = 2;
	private static final int ANOTHER = 3;

	private static final int NUMJOKES = 5;

	private int state = WAITING;
	private int currentJoke = 0;

	private String[] clues = { "Turnip", "Little Old Lady", "Atch", "Who", "Who" };
	private String[] answers = { "Turnip the heat, it's cold in here!",
				     "I didn't know you could yodel!",
				     "Bless you!",
				     "Is there an owl in here?",
				     "Is there an echo in here?" };
	
	public String processInput(String theInput)
	{
	    String theOutput = null;

	    if (state == WAITING)
	    {
		theOutput = "Knock! Knock!";
		state = SENTKNOCKKNOCK;
	    }
	    else if (state == SENTKNOCKKNOCK)
	    {
		if (theInput.equalsIgnoreCase("Who's there?"))
		{
		    theOutput = clues[currentJoke];
		    state = SENTCLUE;
		}
		else
		{
		    theOutput = "You're supposed to say \"Who's there?\"! " +
		    "Try again. Knock! Knock!";
		}
	    }
	    else if (state == SENTCLUE)
	    {
		if (theInput.equalsIgnoreCase(clues[currentJoke] + " who?"))
		{
		    theOutput = answers[currentJoke] + " Want another? (y/n)";
		    state = ANOTHER;
		}
		else
		{
		    theOutput = "You're supposed to say \"" +
		    clues[currentJoke] +
		    " who?\"" +
		    "! Try again. Knock! Knock!";
		    state = SENTKNOCKKNOCK;
		}
	    }
	    else if (state == ANOTHER)
	    {
		if (theInput.equalsIgnoreCase("y"))
		{
		    theOutput = "Knock! Knock!";
		    if (currentJoke == (NUMJOKES - 1))
			currentJoke = 0;
		    else
			currentJoke++;
		    state = SENTKNOCKKNOCK;
		}
		else
		{
		    theOutput = Strings.bye + Strings.dot;
		    state = WAITING;
		}
	    }
	    
	    return theOutput;
	}
    }
}