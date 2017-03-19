using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace autotagger
{
    /*
     * loan_amnt

purpose
car
credit_card
debt_consolidation
home_improvement
house
major_purchase
medical
moving
other
small_business
vacation
wedding

term
 36 months


emp_length
10+ years
< 1 year
10+ years
10+ years
1 year
3 years
8 years
9 years
4 years



home_ownership

RENT, OWN, MORTGAGE, OTHER.

annual_inc

loan_status

Current
Fully Paid
Charged Off
Default

     */


    public enum ExpressionType
    {
        Inclusive,
        Exclusive,
        Undefined
    }

    public class Expression
    {
        public int fieldIdx = -1;
        public string fieldName = "";
        public string value = null;
        public List<string> values = new List<string>();
        public ExpressionType type = ExpressionType.Undefined;
    }

    public class Rule
    {
        public string name = "";
        public List<Expression> expressions = new List<Expression>();
    }

    public class AutoTagger
    {
        private List<Rule> rules = new List<Rule>();

        private List<Expression> parseExpression(string s)
        {

            var t = s.Split('\t');

            if (t.Length < 3)
            {
                Console.WriteLine("Invalid expression for line '{0}'", s);
                Console.WriteLine("Expected format: field keyword value, [value2, value3, ...]");
                return null;
            }

            List<Expression> parsedExpressions = new List<Expression>();

            string fieldname = t[0];
            string keyword = t[1];

            if (keyword == "IS" || keyword == "EQUALS")
            {
                parsedExpressions.Add(new Expression
                {
                    fieldName = fieldname,
                    value = t[2],
                    type = ExpressionType.Inclusive
                });
            }
            else if (keyword == "IS ANY")
            {
                parsedExpressions.Add(new Expression
                {
                    fieldName = fieldname,
                    values = t[2].Split(',').ToList(),
                    type = ExpressionType.Inclusive
                });
            }
            else if (keyword == "ALL EXCEPT" || keyword == "IS NOT")
            {
                string[] tt = t[2].Split(',');
                for (int j = 0; j < tt.Length; j++)
                {
                    parsedExpressions.Add(new Expression
                    {
                        fieldName = fieldname,
                        value = tt[j],
                        type = ExpressionType.Exclusive
                    });
                }
            }
            else
            {
                Console.WriteLine("Invalid keyword for line '{0}'", s);
                Console.WriteLine();
            }

            return parsedExpressions;
        }

        private void readRules(string filename)
        {
            Console.WriteLine("Reading rules from {0}", filename);

            StreamReader sr = new StreamReader(filename);
            Rule def = null;
            while (!sr.EndOfStream)
            {
                string s = sr.ReadLine();
                if (string.IsNullOrWhiteSpace(s)) continue;

                if (s.Contains("\t") && def != null)
                {
                    List<Expression> e = parseExpression(s.Substring(1));
                    if (e != null) def.expressions.AddRange(e);
                }
                else
                {
                    if (def != null && def.expressions.Any()) rules.Add(def);
                    def = new Rule {name = s};
                }
            }

            if (def != null && def.expressions.Any()) rules.Add(def);
            sr.Close();

            Console.WriteLine("Rules read");
        }

        private void printRules()
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("\t\tRULES");
            Console.WriteLine("----------------------------------------");

            for (int i = 0; i < rules.Count; i++)
            {
                Rule rule = rules[i];

                Console.WriteLine("{0}. {1}", i + 1, rule.name);
                foreach (Expression e in rule.expressions)
                {
                    Console.WriteLine("\t{0}\t{1}\t{2}",
                        e.fieldName,
                        e.type,
                        (e.value ?? string.Join(",", e.values)));
                }
            }
            Console.WriteLine();
        }

        private void searchFieldIndexes(string[] headerRow)
        {
            Hashtable h = new Hashtable();
            for (int i = 0; i < headerRow.Length; i++) h[headerRow[i]] = i;

            for (int i = 0; i < rules.Count; i++)
            {
                Rule rule = rules[i];

                foreach (Expression e in rule.expressions)
                {
                    if (h[e.fieldName] != null) e.fieldIdx = (int) h[e.fieldName];
                }
            }
        }

        private bool matchesRule(string[] row, Rule rule)
        {
            foreach (Expression expression in rule.expressions)
            {
                if (expression.type == ExpressionType.Inclusive)
                {
                    if (expression.value != null && row[expression.fieldIdx] != expression.value) return false;
                    if (expression.values.All(value => row[expression.fieldIdx] != value)) return false;
                }
                else if(expression.type == ExpressionType.Exclusive)
                {
                    if (expression.value != null && row[expression.fieldIdx] == expression.value) return false;
                    if (expression.values.Any(value => row[expression.fieldIdx] == value)) return false;
                }
            }

            return true;
        }

        private void printResults(List<int> results)
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("\t\tRESULTS");
            Console.WriteLine("----------------------------------------");

            for (int i = 0; i < rules.Count; i++)
            {
                Rule rule = rules[i];

                Console.WriteLine("{0}. {1}\t\t{2}", i + 1, rule.name, results[i]);
            }

        }

        private void parseFile(string filename)
        {
            Console.WriteLine("Reading data from {0}", filename);

            int lines = 0;

            StreamReader sr = new StreamReader(filename);

            if (!sr.EndOfStream)
            {
                string s = sr.ReadLine();
                if (string.IsNullOrWhiteSpace(s))
                {
                    Console.WriteLine("Failed to receive header row");
                    return;
                }
                searchFieldIndexes(s.Split(','));
            }

            List<int> results = rules.Select(rule => 0).ToList();

            while (!sr.EndOfStream)
            {
                string s = sr.ReadLine();
                if (string.IsNullOrWhiteSpace(s)) continue;

                string[] row = s.Split(',');

                for (int i = 0; i < rules.Count; i++)
                {
                    if (matchesRule(row, rules[i])) results[i]++;
                }

                if (lines%100000 == 0) Console.Write("Processed {0} records\r", lines);
                lines++;
            }
            sr.Close();

            Console.WriteLine("Processed {0} records", lines);

            printResults(results);
        }

        public void run()
        {
            DateTime startTime = DateTime.Now;
            Console.WriteLine("Tagger started");

            readRules("rules.txt");
            printRules();

            parseFile("loan.csv");

            TimeSpan elapsed = DateTime.Now.Subtract(startTime);
            Console.WriteLine("Time elapsed: {0}", elapsed);
        }
    }
}
