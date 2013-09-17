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
	    
	    if (!inputLine.startsWith(
		Strings.arrowLeft +
		Strings.question +
		Strings.xml
	    ))
	    {
		if (!inputLine.startsWith(
		    Strings.arrowLeft +
		    Strings.input +
		    Strings.arrowRight
		) && !inputLine.startsWith(
		    Strings.arrowLeft +
		    Strings.slash +
		    Strings.input +
		    Strings.arrowRight
		)){
		    int start = inputLine.indexOf(Strings.arrowLeft) + 1;
		    int end = inputLine.indexOf(Strings.arrowRight);
		    
		    String key = inputLine.substring(start, end);
		    
		    inputLine = inputLine.split(Strings.arrowRight)[1];
		    
		    end = inputLine.indexOf(Strings.arrowLeft);
		    
		    String value = inputLine.substring(0, end);
		    
		    outputLine = key + Strings.doubleDot + value;
		}
		else if (inputLine.startsWith(
		    Strings.arrowLeft +
		    Strings.input +
		    Strings.arrowRight
		)){
		    count++;
		    
		    outputLine = Strings.input + Strings.doubleDot + count;
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