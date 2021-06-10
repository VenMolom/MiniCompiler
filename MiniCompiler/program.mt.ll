; prolog
@i32 = constant [4 x i8] c"%d\0A\00"
@hex = constant [6 x i8] c"0X%X\0A\00"
@str = constant [4 x i8] c"%s\0A\00"
@double = constant [5 x i8] c"%lf\0A\00"
@true = constant [6 x i8] c"True\0A\00"
@false = constant [7 x i8] c"False\0A\00"
declare i32 @printf(i8*, ...)
define i32 @main()
{

%a = alloca i32
store i32 11, i32* %a
%__1 = load i32, i32* %a
%__2 = load i32, i32* %a
call i32 (i8*, ...) @printf(i8* bitcast ([6 x i8]* @hex to i8*), i32 %__2)

ret i32 0
}
