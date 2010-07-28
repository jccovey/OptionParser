using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OptionParser
{
	public class ParsingException : Exception
	{
		public ParsingException(String option, Exception innerException)
			: base("Invalid values specfied for " + option + " argument.", innerException)
		{

		}
	}
}
