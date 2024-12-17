
: str-synonym1 ( c-addr u -- )
  2dup nextname ['] synonym execute-parsing ;

: copy-names ( "name1 ..." -- )
  begin
    parse-name dup while
      str-synonym1
  repeat
  2drop ;


wordlist constant safe-wordlist \ create new wordlist
safe-wordlist set-current \ set the compilation wordlist as the new wordlist
safe-wordlist >order \ add the new wordlist to the wordlists (make it searchable)

\ those names need to exist
copy-names bye bootmessage

\ remove all other save lists
safe-wordlist 1 set-order
