using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OptionParser
{

	public class Option
	{
		private Action<String> _handler;

		public String Flag { get; private set; }
		public String Name { get; private set; }
		public String Description { get; private set; }
		public bool Empty { get; private set; }

		public Option(String flag, String name, String description, bool empty, Action<String> handler)
		{
			Flag = flag;
			Name = name;
			Description = description;
			Empty = empty;

			_handler = handler;
		}

		public Option(String flag, String name, String description, Action<String> handler)
			: this(flag, name, description, false, handler)
		{

		}

		public void Parse(List<String> values)
		{
			try
			{

				if (Empty)
				{
					_handler(String.Empty);
					return;
				}

				values.ForEach(_handler);

			}
			catch (Exception ex)
			{
				throw new ParsingException(Name, ex);
			}
		}
	}
}