
20 Value game-heigth
70 Value game-width
0 Value y-aliens
Create board game-width 2 + game-heigth * allot
36 Constant num-enemies
Create enemies num-enemies 3 * allot
50 Constant tick \ length of a tick is ms
5 Constant marco-tick \ every <value> tick is a macro-tick
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

: print-win ( -- )
    cr cr cr
    s" ###########################" type cr
    s" #                         #" type cr
    s" #       YOU HAVE WON      #" type cr
    s" #            :)           #" type cr
    s" #                         #" type cr
    s" ###########################" type cr
    cr cr cr
;

: print-lost ( -- )
    cr cr cr
    s" ###########################" type cr
    s" #                         #" type cr
    s" #      YOU HAVE LOST      #" type cr
    s" #            :c           #" type cr
    s" #                         #" type cr
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
    type 0 = if
        addr \ dead
    then
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

: set-enemies { xpos ypos init }
    num-enemies 0 u+do
        enemies i 3 * + \ base offset
        num-enemies 3 / i > if
            dup i num-enemies 3 / mod 2 * 1 + xpos + swap c! \ store x (offset 0)
            dup 1 + 0 ypos + swap c! \ store y (offset 1)
            init true = if
                2 + 1 swap c! \ store type (offset 2) (type=1)
            else
                drop
            then
        then
        num-enemies 3 / i <= num-enemies 3 / 2 * i > and if
            dup i num-enemies 3 / mod 2 * 1 + xpos + swap c! \ store x (offset 0)
            dup 1 + 2 ypos + swap c! \ store y (offset 1)
            init true = if
                2 + 2 swap c! \ store type (offset 2) (type=2)
            else
                drop
            then
        then
        num-enemies 3 / 2 * i <= if
            dup i num-enemies 3 / mod 2 * 1 + xpos + swap c! \ store x (offset 0)
            dup 1 + 4 ypos + swap c! \ store y (offset 1)
            init true = if
                2 + 3 swap c! \ store type (offset 2) (type=3)
            else
                drop
            then
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
    pos y-aliens false set-enemies
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
    max-projectile 0 u+do
        projectile i 2 * 1 + + c@ 255 <> if
            projectile i 2 * 1 + + c@ 1 - projectile i 2 * 1 + + c!
        then
        projectile i 2 * 1 + + c@ -1 = if
            255 projectile i 2 * + c!
            255 projectile i 2 * 1 + + c!
        then
    loop
;

: check-hits { board } ( board -- board )
    \ loop through all the projectiles
    max-projectile 0 u+do
        \ check if projectile is "live"
        projectile i 2 * 1 + + c@ 255 <> if
            \ loop throug all the enemies
            num-enemies 0 u+do
                enemies i 3 * 2 + + c@ 0<> if \ check if enemy is alive
                    enemies i 3 * + c@ \ enemy x
                    projectile j 2 * + c@ \ projectile x
                    =

                    enemies i 3 * 1 + + c@ \ enemy y
                    projectile j 2 * 1 + + c@ \ projectile y
                    =

                    and if
                        \ draw explosion
                        board '#' enemies i 3 * + c@ enemies i 3 * 1 + + c@ write-c-to-pos drop
                        \ remove projectile
                        255 projectile j 2 * + c!
                        255 projectile j 2 * 1 + + c!
                        \ remove enemy
                        0 enemies i 3 * 2 + + c!
                    then
                then
            loop
        then
    loop
    board
;

: add-projectiles { board } ( board -- board )
    max-projectile 0 u+do
        projectile i 2 * + c@ 255 <> if
            board '|'
            projectile i 2 * + c@
            projectile i 2 * 1 + + c@
            write-c-to-pos drop
        then
    loop
    board
;

: check-win  ( -- flag )
    true
    num-enemies 0 u+do
        enemies i 3 * 2 + + c@ 0<> if
            drop false
            leave
        then
    loop
;

: check-lose ( -- flag )
    false
    num-enemies 0 u+do
        enemies i 3 * 2 + + c@ 0<> if
            enemies i 3 * 1 + + c@ game-heigth 1 - = if \ check if alive enemy is on bottom
                drop true
                leave
            then
        then
    loop
;

: game-init ( -- board )
    0 0 true set-enemies board clear-board add-enemies init-projectile
    game-width 2 / \ x pos
    game-heigth 1 - \ y pos
    add-player
;

: handle-input ( key -- )
    dup \ copy because of  checks
    dup 2147483648 = swap 97 = or if \ a | left key
        swap \ get position
        1 - 1 max \ bounded move
        swap
    then
    dup \ copy because of  checks
    dup 2147483649 = swap 100 = or if \ d | rigth key
        swap
        1 + game-width 2 - min \ bounded move
        swap
    then
    dup 2147483650 = swap 119 = or if
        fire-projectile
    then
;
\ change speed of invaders
\ create bunkers
: game-loop { board } ( board -- )
    1 \ alien direction
    0 \ alien position tracker
    game-width 2 / \ player position tracker

    begin \ endless game loop
        marco-tick 0 u+do
            get-input dup if
                ( alien-dir alien-pos player-pos key )
                handle-input
            else
                drop \ no key input
            then
            ( alien-dir alien-pos player-pos )

            board clear-board add-enemies add-projectiles update-projectiles check-hits
            swap dup -rot game-heigth 1 - ( ... board x y ) add-player
            print-board \ draw the board

            \ check win
            check-win if
                drop drop drop \ clearup stack
                true \ set true for win
                unloop exit
            then

            check-lose if
                drop drop drop \ clearup stack
                false \ set false for lose
                unloop exit
            then

            tick sleep
        loop
        \ move the aliens

        -rot move-aliens rot ( alien-dir alien-pos player-pos )
        -rot dup update-alien-pos rot
    again
;

: start-game ( -- )

    print-welcome

    5000 sleep

    clear-window

    game-init game-loop

    clear-window
    if
        print-win
    else
        print-lost
    then
;
