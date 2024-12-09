
20 Value game-heigth
70 Value game-width
0 Value y-aliens
Create board game-width 2 + game-heigth * allot
36 Constant num-enemies
Create enemies num-enemies 3 * allot
100 Constant tick \ length of a tick is ms
10 Constant marco-tick \ every <value> tick is a macro-tick
100 Constant max-projectile
Create projectile max-projectile 2 * allot \ arbitray bound but should work


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

: clear-board { addr } ( addr -- addr )
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
    clear-window
    game-heigth 0 u+do
        game-width 0 u+do
            addr j game-width * i + + c@ emit
        loop
        cr
    loop
;

\ gets the input if not available false
: get-input ( -- c | false )
    key? if
        ekey \ read key

        \ cleanup queue
        begin
            key?
        while
            ekey drop
        repeat
    else
        false
    then
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

: add-player { addr x y } ( addr x y -- addr )
    addr 'o' x y write-c-to-pos
;

: add-enemy { addr x y type }
    type 1 = if
        addr 'T' x y write-c-to-pos
    then
    type 2 = if
        addr 'A' x y write-c-to-pos
    then
    type 3 = if
        addr 'U' x y write-c-to-pos
    then
;

: set-enemies { xpos ypos } ( xpos -- )
    num-enemies 0 u+do
        enemies i 3 * + \ base offset
        num-enemies 3 / i > if
            dup i num-enemies 3 / mod 2 * 1 + xpos + swap c! \ store x (offset 0)
            dup 1 + 0 ypos + swap c! \ store y (offset 1)
            2 + 1 swap c! \ store type (offset 2) (type=1)
        then
        num-enemies 3 / i <= num-enemies 3 / 2 * i > and if
            dup i num-enemies 3 / mod 2 * 1 + xpos + swap c! \ store x (offset 0)
            dup 1 + 2 ypos + swap c! \ store y (offset 1)
            2 + 2 swap c! \ store type (offset 2) (type=2)
        then
        num-enemies 3 / 2 * i <= if
            dup i num-enemies 3 / mod 2 * 1 + xpos + swap c! \ store x (offset 0)
            dup 1 + 4 ypos + swap c! \ store y (offset 1)
            2 + 3 swap c! \ store type (offset 2) (type=3)
        then
    loop
;

: add-enemies { board } ( board -- board )
    num-enemies 0 u+do
        board
        enemies i 3 * + \ base offset
        dup c@ swap \ fetch x
        dup 1 + c@ swap \ fetch y
        2 + c@ \ fetch type
        add-enemy
        drop
    loop
    board
;

: move-aliens { dir pos }
    dir dir pos + \ new pos

    dup -1 = if
        swap drop 1 swap \ change direction to right
        drop pos \ keep original position
        y-aliens 1 + to y-aliens \ move aliens down
    then

    dup game-width num-enemies 3 / 2 * - = if
        swap drop -1 swap \ change direction to left
        drop pos \ keep original position
        y-aliens 1 + to y-aliens  \ move aliens down
    then

;


: update-alien-pos { pos }
    pos y-aliens set-enemies
;

: init-projectile ( -- )
    max-projectile 2 * 0 u+do
        255 projectile i + c!
    loop
;

: fire-projectile { pos } ( pos -- pos )
    max-projectile 0 +do
        projectile i 2 * + c@ 255 = if
            pos projectile i 2 * + c!
            game-heigth 2 - projectile i 2 * 1 + + c!
            leave
        then
    loop
    pos
;

: update-projectiles ( board -- board )

    max-projectile 0 +do
        projectile i 2 * 1 + + c@ 255 <> if
            projectile i 2 * 1 + + c@ 1 - projectile i 2 * 1 + +  .s c!
        then
        projectile i 2 * 1 + + c@ -1 = if
            255 projectile i 2 * + c!
            255 projectile i 2 * 1 + + c!
        then
    loop
;

: add-projectiles { board } ( board -- board )
    max-projectile 0 +do
        projectile i 2 * + c@ 255 <> if
            board '|'


            projectile i 2 * + c@
            projectile i 2 * 1 + + c@
            write-c-to-pos drop
        then
    loop
    board
;

: game-init ( -- board )
    0 0 set-enemies board clear-board add-enemies init-projectile
    game-width 2 / \ x pos
    game-heigth 1 - \ y pos
    add-player
;

: game-loop { board } ( board -- )
    1 \ alien direction
    0 \ alien position tracker
    game-width 2 / \ player position tracker

    begin \ endless game loop
        marco-tick 0 u+do
            get-input dup if

                dup \ copy because of  checks
                dup 2147483648 = swap 97 = or if
                    swap \ get position
                    1 - 1 max \ bounded move
                    swap
                then
                dup \ copy because of  checks
                dup 2147483649 = swap 100 = or if
                    swap
                    1 + game-width 2 - min \ bounded move
                    swap
                then
                dup 2147483650 = swap 119 = or if
                    fire-projectile
                then
            else
                drop \ no key input
            then
            ( alien-dir alien-pos player-pos )

            board clear-board add-enemies add-projectiles update-projectiles swap dup -rot game-heigth 1 -  add-player print-board \ draw the board

            tick sleep
        loop
        \ move the aliens

        -rot move-aliens rot ( alien-dir alien-pos player-pos)
        -rot dup update-alien-pos rot
    again
;
