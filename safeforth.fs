wordlist constant safe-wordlist \ create new wordlist
safe-wordlist set-current \ set the compilation wordlist as the new wordlist
get-order safe-wordlist swap 1 + set-order \ add the new wordlist to the wordlists (make it searchable)

: def-word-by-str ( addr op addr name -- )
        nextname
        find-name name>comp drop \ TODO: add error handeling
        CREATE ,
    DOES> ( executes xt )
        s" new definition" type @ execute ; \ s" new definition" type <- this is just here to show that it works


\ this should later read the commands from a file
: safe-def
    s" .s" 2dup def-word-by-str
    s" bye" 2dup def-word-by-str
    s" dup" 2dup def-word-by-str
;

safe-def \ execute the safe def

\ remove all the other wordslist
safe-wordlist 1 set-order

\ now the only available commands are [.s, bye, dup]
