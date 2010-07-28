using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OptionParser
{
	public class Parser
	{
		private Dictionary<string, List<string>> _option_value_pairs;
		private List<Option> _options;
		private TextWriter _output;

		private String _banner;
		private String[] _help_identifiers;

		private int _max_characters_per_line;

		public Parser(TextWriter output, int max_characters_per_line)
		{
			_option_value_pairs = new Dictionary<string, List<string>>();
			_options = new List<Option>();

			_help_identifiers = new String[] { };

			_output = output;

			_max_characters_per_line = max_characters_per_line;
		}

		public void Banner(String text)
		{
			_banner = text;
		}

		public bool Specified(String option)
		{
			return _options.Any(opt => (opt.Flag.Equals(option) || opt.Name.Equals(option)) && (_option_value_pairs.ContainsKey(opt.Flag) || _option_value_pairs.ContainsKey(opt.Name)));
		}

		public void Option(Option option)
		{
			_options.Add(option);
		}

		public void Option(String option, String name, String description, bool empty, Action<String> handler)
		{
			this.Option(new Option(option, name, description, empty, handler));
		}

		public void Option(String option, String name, String description, Action<String> handler)
		{
			this.Option(option, name, description, false, handler);
		}

		public void Help(String option, String name, String description)
		{
			this.Option(option, name, description, true, s =>
			{
				_output.WriteLine(this.ToUsageString(_banner));
			});

			_help_identifiers = new String[] { option, name };
		}

		public void Parse(String[] options)
		{
			this.MapValuesToOptions(options);

			this.ExecuteHandlersOnOptionValues();
		}

		public String ToUsageString(String description)
		{
			StringBuilder usage = new StringBuilder();
			int longestFlag = _options.Max(o => o.Flag.Length);
			int longestName = _options.Max(o => o.Name.Length);
			int left_margin_width = "    ".Length + longestFlag + longestName;
			int description_column_width = _max_characters_per_line - left_margin_width;

			usage.AppendLine();
			usage.AppendLine(description);
			usage.AppendLine();
			usage.AppendLine("Usage");
			usage.AppendLine();

			foreach (Option o in _options)
			{
				usage.AppendFormat("{0}  {1}  {2}{3}", o.Flag.PadRight(longestFlag), o.Name.PadRight(longestName), String.Join(Environment.NewLine + String.Empty.PadRight(left_margin_width), WordWrap.Wrap(o.Description, description_column_width).ToArray()), Environment.NewLine);
			}

			usage.AppendLine();

			return usage.ToString();
		}

		private void MapValuesToOptions(String[] options)
		{
			foreach (string o in options)
			{
				if (_options.Any(option => option.Flag == o || option.Name == o))
				{
					_option_value_pairs.Add(o, new List<string>());

					if (_help_identifiers.Contains(o))
					{
						_options.Last().Parse(new List<String>());
						return;
					}

					continue;
				}

				_option_value_pairs.Last().Value.Add(o);
			}
		}

		private void ExecuteHandlersOnOptionValues()
		{
			try
			{
				foreach (Option o in _options.Where(r => _option_value_pairs.ContainsKey(r.Flag) || _option_value_pairs.ContainsKey(r.Name)))
				{
					o.Parse(_option_value_pairs.ContainsKey(o.Flag) ? _option_value_pairs[o.Flag] : _option_value_pairs[o.Name]);
				}
			}
			catch (ParsingException ex)
			{
				_output.WriteLine(ex.Message);
			}
		}
	}
}