
\ for reading files
0 Value fd-in
256 Constant max-line
Create line-buffer  max-line 2 + allot

: str-synonym1 ( c-addr u -- )
  2dup nextname ['] synonym execute-parsing ;

: copy-names ( "name1 ..." -- )
  begin
    parse-name dup while
      str-synonym1
  repeat
  2drop ;

: copy-names-from-file ( addr u -- )
  r/o open-file throw ( file ptr ) to fd-in
  begin
    line-buffer max-line fd-in read-line throw
  while
    line-buffer swap str-synonym1
  repeat

;

: copy-from-all-arg-files ( ... addr u -- )
  depth 2 / 0 u+do \ loop through the number of strings on the stack1 of 0 is encountered
    dup 0 = if
      drop
      leave
    endif
    copy-names-from-file
    drop
  loop

;

wordlist constant safe-wordlist \ create new wordlist
safe-wordlist set-current \ set the compilation wordlist as the new wordlist
safe-wordlist >order \ add the new wordlist to the wordlists (make it searchable)

\ those names need to exist
copy-from-all-arg-files
copy-names bye bootmessage

\ create new user wordlist (to prevent redefinition exploit)
wordlist constant user-wordlist \ create new wordlist
user-wordlist set-current \ set the compilation wordlist as the new wordlist


\ remove all other save lists
user-wordlist safe-wordlist 2 set-order
