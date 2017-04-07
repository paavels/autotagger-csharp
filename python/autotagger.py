"""This application does simplified categorization of CSV rows based on defined rules."""
#!/usr/bin/env python

import csv
from collections import namedtuple

TAGGING_RULES = []
#class Point(namedtuple('Point', 'x y')):
#     __slots__ = ()
#     @property
#     def hypot(self):
#         return (self.x ** 2 + self.y ** 2) ** 0.5
#    def __str__(self):
#        return 'Point: x=%6.3f  y=%6.3f  hypot=%6.3f' % (self.x, self.y, self.hypot)
#

Expression = namedtuple("Expression", "fieldIdx fieldName value type")
Rule = namedtuple("Rule", "name expressions")

def parse_expression(expression):
    """Parses expression from string"""

    print("Expression parse")

    return [1, 2]

    #return

def read_rules(filename):
    """Reads rules from file"""

    with open(filename, "r") as file_pointer:

        print("Reading rules from " + filename)

        rule = None

        for line in file_pointer:
            line = line.strip()

            if not line:
                continue

            if rule and "\t" in line:
                expr = parse_expression(line)
                if expr:
                    rule.expressions.append(expr)

            else:
                if rule and rule.expressions:
                    TAGGING_RULES.append(rule)

                rule = Rule(line, [])

            #print(line)

        if rule and rule.expressions:
            TAGGING_RULES.append(rule)

        file_pointer.close()

    return

def print_rules():
    """Prints rules on screen"""

    for rule in TAGGING_RULES:
        print(rule.name)

    return

read_rules("rules.txt")
print_rules()


