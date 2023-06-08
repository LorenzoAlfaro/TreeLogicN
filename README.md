# TreeLogicN

The N in tree logic stands for Nugget.

## Introduction
This project started when I discovered the power of recursiveness and data structures like graphs and tries.
All my formal knowledge in programming up to that point was a C/C++ introductory class in college that went up only to the knowledge of linked lists.
## The problem solved
It was during my time in a job role where my work was very manual and tedious. Of course I started seeing how I could automate that.
Basically my duty was to go thru a schematic and find all the devices connected thru a network. I had to manually read the schematic and jump around many pages to map out the complete graph for a given point.
One day I sat down in a break room and ponder how to implement an automatic solution and realized that it was a recursive problem.
## The gist of the algorithm
My basic idea was:
A function that tracks all the ‘exits’ 
(that is how I visualized a list edges that excluded the edge that led me to that node) from a node and returns a list of all the valid ‘exits’ and then calls himself again for each ‘exit’ 
and adds the result of each recursive call to his return list. 
My brain even hurts thinking about it.
It took several months of refining. Improvements, avoid the use of global variables to make it reusable and modular. 
Like adding a parameter that included nodes already visited to avoid stack overflows. 
Creating a transformer that took the ‘list of lists’ representation of the trie stored in memory in save it as a JSON file. 
This was my introduction to the JSON format. With the collaboration of a co-worker who was working in Web Viewer (using the [D3]( https://d3js.org/) javascript library)  for schematics 
we could take my JSON and drop it in his web page and it would create a simple view of the completed network.

## The rewards
Manual work went from a couple of hours to just 20 minutes using the tools that we made.
The next part of the solution was to take all those ‘threads’ of explored paths and create something resembling a tree. Many years later I learned I was working with tries. And even realized that I had implemented an algorithm to convert the list of paths to a trie (even though it was not the most optimal)


