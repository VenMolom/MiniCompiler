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

br i1 1, label %__2, label %__1
__2:
br label %__3
__1:
br label %__3
__3:
%__4 = phi i1 [ 1, %__2], [0, %__1]
br i1 %__4, label %__5, label %__6
__5:
call i32 (i8*, ...) @printf(i8* bitcast ([5 x i8]* @true to i8*))
br label %__7
__6:
call i32 (i8*, ...) @printf(i8* bitcast ([6 x i8]* @false to i8*))
br label %__7
__7:
br i1 1, label %__9, label %__8
__9:
br label %__10
__8:
br label %__10
__10:
%__11 = phi i1 [ 1, %__9], [1, %__8]
br i1 %__11, label %__12, label %__13
__12:
call i32 (i8*, ...) @printf(i8* bitcast ([5 x i8]* @true to i8*))
br label %__14
__13:
call i32 (i8*, ...) @printf(i8* bitcast ([6 x i8]* @false to i8*))
br label %__14
__14:
br i1 0, label %__16, label %__15
__16:
br label %__17
__15:
br label %__17
__17:
%__18 = phi i1 [ 1, %__16], [0, %__15]
br i1 %__18, label %__19, label %__20
__19:
call i32 (i8*, ...) @printf(i8* bitcast ([5 x i8]* @true to i8*))
br label %__21
__20:
call i32 (i8*, ...) @printf(i8* bitcast ([6 x i8]* @false to i8*))
br label %__21
__21:
br i1 0, label %__23, label %__22
__23:
br label %__24
__22:
br label %__24
__24:
%__25 = phi i1 [ 1, %__23], [1, %__22]
br i1 %__25, label %__26, label %__27
__26:
call i32 (i8*, ...) @printf(i8* bitcast ([5 x i8]* @true to i8*))
br label %__28
__27:
call i32 (i8*, ...) @printf(i8* bitcast ([6 x i8]* @false to i8*))
br label %__28
__28:

ret i32 0
}
