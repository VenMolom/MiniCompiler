; prolog
@i32 = constant [3 x i8] c"%d\00"
@hex = constant [5 x i8] c"0X%X\00"
@hexread = constant [3 x i8] c"%X\00"
@double = constant [4 x i8] c"%lf\00"
@true = constant [5 x i8] c"True\00"
@false = constant [6 x i8] c"False\00"


declare i32 @printf(i8*, ...)
declare i32 @scanf(i8*, ...)

define i32 @main()
{

%__1 = and i32 15, 40
call i32 (i8*, ...) @printf(i8* bitcast ([3 x i8]* @i32 to i8*), i32 %__1)
%__2 = or i32 15, 40
call i32 (i8*, ...) @printf(i8* bitcast ([3 x i8]* @i32 to i8*), i32 %__2)

ret i32 0
}
