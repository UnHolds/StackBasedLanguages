
256 Constant max-line
Create line1  max-line 2 + allot
Create line2  max-line 2 + allot
Create line3  max-line 2 + allot
Create line4  max-line 2 + allot
Create line5  max-line 2 + allot
0 Value fd-in
0 Value fd-out

: open-input ( addr u -- )
    r/o open-file throw to fd-in
;

: open-output ( addr u -- )
    2dup
    r/w bin open-file
    dup 0= if
        throw to fd-out
        2drop
    else
        2drop 2dup r/w create-file throw drop
        r/w bin open-file throw to fd-out
    then
;

 \ puts the number of lines at the top of the stack
: numberOfLines { addr fileName }
    addr fileName open-input
    0
    begin
        pad max-line fd-in read-line throw drop
    while
        1 +
    repeat
    fd-in close-file throw
;


\ reads the line at lineNum and puts the line and the length on top of the stack
: readLineAt { addr fileName lineNum mem }
    addr fileName open-input
    0
    lineNum 1 + 0 +do
        drop
        mem max-line fd-in read-line throw drop
    loop
    mem swap \ swap the memory with line with
    fd-in close-file throw
;

\ append line to file
: appendLineToFile { addr1 fileName addr2 line }
    addr1 fileName open-output

    fd-out file-size throw \ get the file size
    fd-out reposition-file throw
    addr2 line fd-out write-line throw
    fd-out close-file throw
;

\ split string on delimiter and the str at splitted array index
: splitStrOnDelAndGetStrAtIdx { addr mem delimiter idx }
    0 \ length
    addr \ address
    idx \ store idx

    dup 0= if \ fix if idx 0
        swap 1 - swap rot 1 + -rot
    then

    mem 0 +do
        addr i + c@ \ fetch char
        delimiter =  if \ check if char matches delimter
            1 - \ decrease idx
        then

        \ increae the address if the idx is greater 0
        dup 0> if
            swap 1 + swap
        then

        \ increase the length if the idx is 0
        dup 0= if
            rot 1 + -rot
        then

    loop
    drop 1 + swap 1 - \ remove the idx counter fix addr (remove leading delimiter) and swap addr + length

;

: splitStrOnDelAndRemoveStrAtIdx { addr mem delimiter idx }

    idx

    dup 0= if \ special case for idx
        addr 1 - swap \ store copy addr
        mem 1 - swap \ store length
    then

    mem 0 +do

        addr i + c@ \ fetch char
        delimiter =  if \ check if char matches delimter
            1 - \ decrease idx
            dup 0= if
                addr i + swap \ store copy addr
                mem swap \ store length
            then
        then

        dup 0= if
            swap 1 - swap \ decreases length
        then

        dup 0< if
            rot dup \ get the addr and dup
            addr i + c@ \ fetch current char
            swap c! \ store the current char at new loc
            1 + -rot \ increase and rot
        then
    loop
    drop swap drop addr swap
;

: joinStrOn { addr1 str1 addr2 str2 addr3 dst joinChr }
    addr3 \ store result addr
    addr1 \ store first string addr
    str1 0 +do
        dup c@ \ read char from first addr
        rot dup -rot c! \ store the char in the new memory
        1 + swap 1 + \ increase point
    loop
    drop

    dup joinChr swap c! 1 + \ store join char

    addr2 \ store second string addr
    str2 0 +do
        dup c@ \ read char from first addr
        rot dup -rot c! \ store the char in the new memory
        1 + swap 1 + \ increase point
    loop
    2drop

    addr3
    str1 str2 1 + + \ calculate length
;

: join { addr1 file1 idx1 addr2 file2 idx2 addr3 outFile }

    addr1 file1 numberOfLines
    0 +do
        addr1 file1 i line1 readLineAt \ read file 1
        ',' idx1 splitStrOnDelAndGetStrAtIdx

        addr2 file2 numberOfLines
        0 +do
            2dup \ duplicate file 1 split

            addr2 file2 i line2 readLineAt \ read file 2
            ',' idx2 splitStrOnDelAndGetStrAtIdx

            compare 0= if
                \ cr cr s" match " type cr
                addr1 file1 j line3 readLineAt \ read file 1 <- store in new memory (easier)
                addr2 file2 i line4 readLineAt \ read file 2 <- store in new memory (easier)
                ',' idx2 splitStrOnDelAndRemoveStrAtIdx

                line5 max-line ',' joinStrOn \ join the strings

                addr3 outFile 2swap appendLineToFile \ write to file

            then
        loop
        2drop
    loop
;
