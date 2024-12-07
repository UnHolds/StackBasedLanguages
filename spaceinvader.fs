
10 Constant game-heigth
50 Constant game-width
Create board game-width 2 + game-heigth * allot

: clear-window ( -- )
    10 0 u+do cr loop
;

: print-welcome ( -- )
    cr cr cr
    s" ###########################" type cr
    s" #                         #" type cr
    s" #      GFORTH INVADERS    #" type cr
    s" #                         #" type cr
    s" #         by Alex         #" type cr
    s" ###########################" type cr
    cr cr cr
;

: write-c-to-pos ( addr c x y -- addr)
    game-width * + rot dup -rot + rot swap ( addr c offset-addr )
    c! \ write char
;

: init-board { addr } ( addr - addr )
    addr
    \ clear board
    game-heigth 0 u+do
        game-width 0 u+do
            bl i j write-c-to-pos
        loop
    loop
    \ draw the side walls
    game-heigth 0 u+do
        '#' 0 i write-c-to-pos
        '#' game-width 1 - i write-c-to-pos
    loop
;

\ prints the game board
: print-board { addr }
    game-heigth 0 u+do
        game-width 0 u+do
            addr j game-width * i + + c@ emit
        loop
        cr
    loop
;

\ gets the input if not available false
: get-input ( -- c | false )
    key? if ekey else false then
;

: sleep ( ms -- )
    1000 * utime throw +
    begin
        dup utime throw >
    while
        \ do nothing
    repeat
    drop
;

: game-loop ( -- )

;
