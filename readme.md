# AutoTagger
This application does simplified categorization of CSV rows based on defined rules.
It is rather than *quick and dirty* implementation than complete and foolproof package. 

## Rules
Categorization rules are stored in file rules.txt

File example
```
US Northeast Garbage Renters
	addr_state	IS ANY	PA,NY,NJ,VT,RI,NH,MA,ME,CT
	grade	IS NOT	A,B,C
	home_ownership	IS ANY	RENT,MORTGAGE
```

File format

First row is name of rule
Followed by rows of rule definitions
Empty line defines end of rule

```
**rule**
	\t	<field_name>	\t	<keyword>	\t	<value or values>

```

## Performance

1. Single thread / Array based run: 13.52sec



## Test data used
https://www.kaggle.com/wendykan/lending-club-loan-data
