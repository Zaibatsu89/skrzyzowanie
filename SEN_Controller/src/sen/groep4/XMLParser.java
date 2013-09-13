package sen.groep4;

import java.util.ArrayList;
import java.util.List;

public class XMLParser
{
    int count = 0;
    
    public List<String> parse(List<String> input)
    {
	List<String> output = new ArrayList();
	
	for (int i = 0; i < input.size(); i ++)
	{
	    String inputLine = input.get(i);
	    String outputLine = new String();
	    
	    if (!inputLine.startsWith("<?xml"))
	    {
		if
		(
		    !inputLine.startsWith("<input>") &&
		    !inputLine.startsWith("</input>")
		)
		{
		    int start = inputLine.indexOf("<") + 1;
		    int end = inputLine.indexOf(">");
		    
		    String key = inputLine.substring(start, end);
		    
		    inputLine = inputLine.split(">")[1];
		    
		    end = inputLine.indexOf("<");
		    
		    String value = inputLine.substring(0, end);
		    
		    outputLine = key + ":" + value;
		}
		else if (inputLine.startsWith("<input>"))
		{
		    count++;
		    
		    outputLine = "input:" + count;
		}
	    }
	    
	    if (!outputLine.isEmpty())
	    {
		output.add(outputLine);
	    }
	}
	
	return output;
    }
}