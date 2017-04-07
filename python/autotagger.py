"""This application does simplified categorization of CSV rows based on defined rules."""
#!/usr/bin/env python

import csv
from collections import namedtuple
import time
import sys

#class Point(namedtuple('Point', 'x y')):
#     __slots__ = ()
#     @property
#     def hypot(self):
#         return (self.x ** 2 + self.y ** 2) ** 0.5
#    def __str__(self):
#        return 'Point: x=%6.3f  y=%6.3f  hypot=%6.3f' % (self.x, self.y, self.hypot)
#

Expression = namedtuple("Expression", "field value inclusive")
Rule = namedtuple("Rule", "name expressions")

def parse_expression(expression):
    """Parses expression from string"""

    expr_parts = expression.lstrip().split("\t")

    if len(expr_parts) < 3:
        print("Invalid expression for line '{0}'".format(expression))
        print("Expected format: field keyword value, [value2, value3, ...]")

        return None

    keyword = expr_parts[1]

    if keyword == "IS" or keyword == "EQUALS" or keyword == "IS ANY":
        return Expression(expr_parts[0], "," + expr_parts[2] + ",", True)

    elif keyword == "ALL EXCEPT" or keyword == "IS NOT":
        return Expression(expr_parts[0], "," + expr_parts[2] + ",", False)

    print("Invalid keyword for line '{0}'".format(expression))
    print()

    return None

def read_rules(filename):
    """Reads rules from file"""

    with open(filename, "r", encoding="utf-8-sig") as file_pointer:

        print("Reading rules from " + filename)

        rules = []
        rule = None

        for line in file_pointer:
            line = line.rstrip()

            if not line:
                continue

            if line.startswith("\t"):
                if not rule:
                    print("Found expression without rule, skipping. Expression: " + line)
                    continue

                expr = parse_expression(line)

                if expr:
                    rule.expressions.append(expr)

            else:
                rule = Rule(line, [])
                rules.append(rule)

            #print(line)

        file_pointer.close()

    return [r for r in rules if r.expressions]

def check_rule(entry, rule):
    """Checks entry for rule"""

    for expr in rule.expressions:
        val_to_check = "," + entry[expr.field] + ","

        if expr.inclusive:
            if val_to_check not in expr.value:
                return False
        else:
            if val_to_check in expr.value:
                return False

    return True

def parse_file(filename, rules):
    """Parses incoming file and tags the rows according rules"""

    print("Reading data from " + filename)

    results = {}
    for rule in rules:
        results[rule.name] = 0

    with open(filename, "r", encoding="utf-8-sig") as file_pointer:
        reader = csv.DictReader(file_pointer)
        lines = 0

        for entry in reader:
            for rule in rules:
                rule_passed = check_rule(entry, rule)
                results[rule.name] = results[rule.name] + 1 if rule_passed else 0

            if lines % 100000 == 0:
                print("Processed {0} records\r".format(lines))
            lines += 1

#            print(entry)
#            return results

    print("Processed {0} records\r".format(lines))

    return results

def print_results(results):
    """Prints results on screen"""

    print("----------------------------------------")
    print("\t\tRESULTS")
    print("----------------------------------------")

    i = 0
    for key, value in results.items():
        i += 1
        print("{0}. {1}\t\t{2}".format(i, key, value))

    return

def print_rules(rules):
    """Prints rules on screen"""

    print("----------------------------------------")
    print("\t\tRULES")
    print("----------------------------------------")

    for rule in rules:
        print(rule.name)
        for expr in rule.expressions:
            mode = "INCLUSIVE" if expr.inclusive else "EXCLUSIVE"
            print("\t{0}\t{1}\t{2}".format(expr.field, mode, expr.value))
        print()

    print("{0} rules".format(len(rules)))

    return

def main(argv):
    """Main application block"""

    start_time = time.process_time()
    print("Tagger started")

    rules = read_rules("rules.txt")
#    print_rules(rules)

    results = parse_file("loan.csv", rules)
    print_results(results)

    elapsed = time.process_time() - start_time
    print("Time elapsed: {0}".format(elapsed))

if __name__ == "__main__":
    main(sys.argv)
