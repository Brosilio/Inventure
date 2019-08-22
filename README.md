# Inventure
Inventure: Invent your own text adventure.

Inventure is designed to be an easy-to-use format for creating your own text adventures. It's medium-easy to parse and provides plenty of features to create complex text adventure games and interactive "choose your own story" things.

## Getting Started
Currently there are no pre-made parsers available, however, that will change soon.
Parsers in the works:
 - [ ] C#
 - [ ] JavaScript
 - [ ] Java
 - [ ] Python
 - [ ] C / C++
 
 If you have written a parser for an unlisted language, make a pull request so others can use your parser too!
 
 ## The Format

 ### Basics
 The Inventure format is designed to be easy to generate or write by hand. It certainly isn't the easiest to parse, but it's doable. The files are plaintext and contain a mixture of `directives` and `text`. Parsers should parse top-down.

Inventure files usually have the extension `.ivt`. Parsers can pre-parse the entire file and create a sort of abstract syntax tree, or parse them on-the-fly.

 A `directive` is an instruction to the parser, and `text` is for the user to read. Anything that is not explicitly a directive will be sent to the renderer to be rendered to the screen so the user can read it.

 Example:
 ```
 This is some plaintext garbage.
 @directve { param };
 Here's some more plaintext.
 ```

### Directive Syntax
All directives in Inventure begin with an "at" symbol `@`, are single words, and are followed by a semicolon. They can have parameters, which are encased in curly braces `{ }`. Parsers must support multiple `directive` statements per line.

Example:
```d
@directive { param_one, param_two }; @directive;
@directive { foo: "bar", num: 50 };
```

There is a set of directives that all parsers must handle correctly. These are the base directives.

### Base Directives
This is the set of base directives that all parsers must digest properly.

#### @meta
The `meta` directive is used to set metadata that relates to the current Inventure file. Maybe an author's name, or contact information.
```d
@meta { "Author Name", "Ben Richards" };
@meta { "Author Email", "brichards@ics.co"};
@meta { "Copyright", "yes" };
```

There are a few reserved names for meta values. You can specify these values to configure the parser and stuff like that.

TODO: the list of things

#### @var
The `var` directive is used to set a variable that can be accessed anywhere else in the file. Variable names must not contain spaces and cannot start with a number. Variables can be `strings` or `numbers`. The type should be automatically determined by the parser. Variable values can be printed directly by surrounding the name of the variable with pointy brackets `< >`, like this: `<playername>`. This can be used inside plaintext areas to create a dynamic experience.

```
@var { var_name, "value" };

This is some plaintext stuff. Here is the value of var_name: <var_name>.
```

#### @expunge
The `expunge` directive is used to remove a variable from the variable store.
```d
@expunge { var_name };
```

#### @location
The `location` directive is used to define a named location at which the parser can jump to, upon request. In programming languages, this is commonly called a label. No two `location` directives can have the same name. To jump to a location, use the `goto` directive.

```d
@location { "name" };
```

#### @goto
The `goto` directive can be used to immediately jump around the file. Parsing should continue when the target `location` is reached.

```d
@goto { "name" };
```

#### @title
The `title` directive is a way to show text to the user in a specially formatted way.

```d
@title { "Chapter 1" };
```

#### @read
The `read` directive is used to get text input from the user and then store it in the specified variable. If the variable does not exist, it is created.
```d
@read { var_to_store_result_in };
```

#### @option
The `option` directive is the prime directive for creating interactive stories. It is a list of options that the user can choose from, and an accompanying list of `location` names to jump to when the user chooses an item from the list. The input system will ask the user what they want to do until they select a valid option. It's up to you to decide if the user can see all the available options, or if they have to guess.

```d
@option
{
    "The thing the user has to type": "the location to jump to when the user types the thing",
    "Another thing": "another location name"
};
```

The `option` directive can contain as many options and locations as required.

#### @jif (jump if)
Like many programming languages, Inventure has an acceptably powerful `if` statement. It is equivilent to saying, "if this is true, jump to this label."
You may have noticed that this is `jif` and not `if`. That's because a `jif` directive takes a single argument, which is the name of a label to jump to if the condition is met.
The REAL `if` directive is different, find the example on this page to see what that one does.

Syntax:
```d
@jif ( expression ) { "location to go to" };
```

Example:
```d
@jif (var = "shnazzle") { "name of label to jump to" };
@jif (var < 50) { "VarIsLessThan50" };
```

#### @if
This is the real `if` directive. Like the `jif` directive, it does something only if a condition is met. However, the `jif` statement can only jump to a single label if the condition is met.
The `if` directive can contain a block of stuff to do if the condition is met. The stuff in the block can be directives or plaintext. Or whatever.

Example:
```
This is some plaintext that will be printed to the user.
Here's a bit more plaintext.
@if(var == "eat pant")
{
    Here's some plaintext that will only be shown if the condition is met.
    You could even replicate the functionality of a @jif directive.
    @goto { "location" };
}
```


#### @expr
This is reserved for future expansions.

#### Math Directives
There are a few directives that can be used to do math on variables (obviously only if they're a number). They can be used to keep track of the amount of loops something has done, or something. Be creative.

#### @incr
The `incr` directive is used to increment the specified variable by the specified amount.
```d
@incr { var_name, 69 };
```

#### @decr
The `decr` directive is used to decrement the specified variable by the specified amount.
```d
@decr { var_name, 420 };
```

#### @mul
The `mul` directive is used to multiply the specified variable by the specified number.
```d
@mul { var_name, 2 };
```

#### @div
The `div` directive is used to divide the specified variable by the specified divisor.
```d
@div { var_name, 2 };
```