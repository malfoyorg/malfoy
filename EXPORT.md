# *export*

Takes an *email:hash* file(s) and combines it with the output file from a hashcat [hashcat](https://github.com/hashcat/hashcat) mode 9 attack, to create the following outputs.

- *name*.left.txt - The remaining *email:hash* pairs which were not found in the output file.
- *name*.found.txt - A list of *hash:plain* pairs found in both input files.
- *name*.plains.txt - A list of *email:plain* pairs created by combining the two input files

## Usage

`meta export hashespath outputpath [options]`
&nbsp;<br>
&nbsp;<br>

| Option | Description |
| :--- | :--- |
| --remove-hash | Path to the file to remove any matched hashes from Usual *name*.hash|
| --remove-word | Path to the file (usually *name*.word) to remove associated words when used in conjunction with --remove-hash. |
| --shuck| The path to the file that will be used to convert founds back to plains.  |
| --ignore-salt| Do not compare salts when matching hashes. Default is false. |
| <img width=350> | |

 
## Examples
 
In the documentation for (CATALOG.md) and (LOOKUP.md) we used the *breach.txt* file as a list of *email:plain* value we wanted to crack

>alice.smith<span>@icloud.com:$2a$10$XsDGiVuwaoYP8uGDoleDmuWV9s4MtMCn1OWzV3PEEFL4gtYVroNW2
>alice1974<span>@apple.com:$2a$10$myx7zGGnlbgRxyaPhF0NwuYkJuQ0qSHuShRpL8bQVfgGHQaIf4.Hy
>jim<span>@acmesec.org:$2y$10$uhNbQS6F9bfHMPp.yc6BB.R.XfwqFF3/b0lxS23mNOpkM8TEYyPrC

with an output file from Hashcat containing the following *hash:plain* pairs. We can run the *export* command to associate the emails with the cracked plains, and to clean up any duplicated from hashcat. The command below creates the following outputs: 

`meta export breach.txt breach.output.txt`
&nbsp;<br>
&nbsp;<br>
	
*breach.left.txt*
>jim<span>@acmesec.org:$2y$10$uhNbQS6F9bfHMPp.yc6BB.R.XfwqFF3/b0lxS23mNOpkM8TEYyPrC
	
*breach.found.txt*
>$2a$10$XsDGiVuwaoYP8uGDoleDmuWV9s4MtMCn1OWzV3PEEFL4gtYVroNW2:password
>$2a$10$myx7zGGnlbgRxyaPhF0NwuYkJuQ0qSHuShRpL8bQVfgGHQaIf4.Hy:test9

*breach.plains.txt*
>alice.smith<span>@icloud.com:password<br>
>alice1974<span>@apple.com:test9			

	
### Remove matched values from *.hash* and *.word* files

In our previous example, the breach.hash and the breach,word files were unchanged. If they contained additional hashes we wanted to crack in another attack, or if our attack was part of multiple sessions generated by a (Lookup)[LOOKUP.md], then we would want to remove matched hashes and words from these files. 
	
The following command shows how we would achieve this:

`meta export breach.txt breach.output.txt --remove-hash breach.hash --remove-word breach.word`
