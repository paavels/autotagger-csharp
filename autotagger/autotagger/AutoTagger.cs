using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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


grade
A-G

sub_grade
A1-A5 B1-B5...

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

            switch (keyword)
            {
                case "EQUALS":
                    break;
            }

            Expression e = new Expression();
            e.fieldName = t[0];

            if (keyword == "IS" || keyword == "EQUALS" || keyword == "ONE OF")
            {
                parsedExpressions.Add(new Expression
                {
                    fieldName = fieldname,
                    type = ExpressionType.Inclusive
                });
            }
            else if (keyword == "ALL EXCEPT" || keyword == "EXCLUDING")
            {
                parsedExpressions.Add(new Expression
                {
                    fieldName = fieldname,
                    type = ExpressionType.Exclusive
                });
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
                    List<Expression> e = parseExpression(s.Replace("\t", ""));
                    if (e != null) def.expressions.AddRange(e);
                }

                if (def != null && def.expressions.Any()) rules.Add(def);
                def = new Rule { name = s };
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
                    Console.WriteLine("\t{0}", e.fieldName);
                }
            }
            Console.WriteLine();
        }

        private void readData(string filename)
        {
            Console.WriteLine("Reading data from {0}", filename);

            int lines = 0;

            StreamReader sr = new StreamReader(filename);
            while (!sr.EndOfStream && lines < 5)
            {
                string s = sr.ReadLine();
                if (string.IsNullOrWhiteSpace(s)) continue;
                Console.WriteLine(s);
                lines++;
            }
            sr.Close();

            Console.WriteLine("Read {0} lines", lines);
        }

        public void run()
        {
            DateTime startTime = DateTime.Now;
            Console.WriteLine("Tagger started");

            readRules("rules.txt");
            printRules();

            //readData("loan.csv");

            TimeSpan elapsed = DateTime.Now.Subtract(startTime);
            Console.WriteLine("Time elapsed: {0}", elapsed);
        }
    }
}
