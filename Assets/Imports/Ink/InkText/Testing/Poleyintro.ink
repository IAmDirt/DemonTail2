
//VAR FirstAttraction = ""
//~FirstAttraction ="Gambling den"
# Speaker: Poley
# Portrait: Poley_Happy
Hello, and welcome to the CARNIVAL OF TEMPTATION!

I am your guide, Poley!
-> main

=== main ===
Do you have any Questions ?# Portrait: Poley_Neutral
 * [Where is the toilet?]
 We dont have any.
 -> main
 * [Why are you a pole?]# Portrait: Poley_Sad
 I was curesd for asking too many questions..
 -> main
 * [I dont have any questions]# Portrait: Poley_Angry
well aren't you that smartes person alive.
 -> Carnival
 
 *->
 So no more questions ?# Portrait: Poley_Happy
 lets continue!
 -> Carnival

=== Carnival ===
Let me show you around!
we have the sugery mountains to the east, and the gambling district ot the west.
which one to visit first?# Portrait: Poley_Neutral
    * [Gambling den]
        ->chosen("Gambling den")
    * [Sugary mountains]
        ->chosen("Sugary mountains")
    
    === chosen(FirstAttraction) ===
    ok, lets go to the {FirstAttraction} First!# Portrait: Poley_Happy


    -> END