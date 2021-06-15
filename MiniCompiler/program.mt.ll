; prolog
@i32 = constant [3 x i8] c"%d\00"
@hex = constant [5 x i8] c"0X%X\00"
@hexread = constant [3 x i8] c"%X\00"
@double = constant [4 x i8] c"%lf\00"
@true = constant [5 x i8] c"True\00"
@false = constant [6 x i8] c"False\00"

@__1 = constant [12 x i8] c"First step\0A\00"
@__2 = constant [17 x i8] c"    Second step\0A\00"
@__3 = constant [20 x i8] c"        Third step\0A\00"
@__4 = constant [17 x i8] c"            ...\0A\00"
@__5 = constant [46 x i8] c"                You went to infinity and ...\0A\00"
@__6 = constant [20 x i8] c"You failed anyway.\0A\00"

declare i32 @printf(i8*, ...)
declare i32 @scanf(i8*, ...)

define i32 @main()
{

%cond = alloca i1
%num = alloca i32
%dec = alloca double
%temp = alloca double
store i32 0, i32* %num
%__7 = sitofp i32 0 to double
store double %__7, double* %temp
store double %__7, double* %dec
%__8 = load i32, i32* %num
%__9 = load i32, i32* %num
%__10 = icmp sle i32 %__8, %__9
%__11 = load double, double* %dec
%__12 = load double, double* %dec
%__13 = fcmp ogt double %__11, %__12
%__15 = xor i1 %__13, 1
%__16 = icmp eq i1 %__10, %__15
store i1 %__16, i1* %cond
%__20 = load i1, i1* %cond
%__22 = xor i1 %__20, 1
br i1 %__22, label %__17, label %__18
__17:
ret i32 0
br label %__19
__18:
br label %__19
__19:
%__23 = load i1, i1* %cond
br i1 %__23, label %__24, label %__25
__24:
call i32 (i8*, ...) @printf(i8* bitcast ([5 x i8]* @true to i8*))
br label %__26
__25:
call i32 (i8*, ...) @printf(i8* bitcast ([6 x i8]* @false to i8*))
br label %__26
__26:
%__27 = load i1, i1* %cond
br i1 %__27, label %__28, label %__29
__29:
br label %__30
__28:
%__32 = load i32, i32* %num
%__34 = xor i32 %__32, -2147483648
%__35 = load i32, i32* %num
%__37 = xor i32 %__35, -2147483648
%__38 = icmp eq i32 %__34, %__37
br i1 %__38, label %__40, label %__39
__40:
br label %__41
__39:
%__43 = load i32, i32* %num
%__44 = sdiv i32 %__43, 0
%__45 = icmp eq i32 %__44, 0
br label %__41
__41:
%__42 = phi i1 [ 1, %__40], [%__45, %__39]
br label %__30
__30:
%__31 = phi i1 [ 0, %__29], [%__42, %__28]
store i1 %__31, i1* %cond
%__49 = load i1, i1* %cond
%__51 = xor i1 %__49, 1
br i1 %__51, label %__46, label %__47
__46:
ret i32 0
br label %__48
__47:
br label %__48
__48:
%__52 = load i1, i1* %cond
br i1 %__52, label %__53, label %__54
__53:
call i32 (i8*, ...) @printf(i8* bitcast ([5 x i8]* @true to i8*))
br label %__55
__54:
call i32 (i8*, ...) @printf(i8* bitcast ([6 x i8]* @false to i8*))
br label %__55
__55:
%__56 = load i1, i1* %cond
br i1 %__56, label %__57, label %__58
__58:
br label %__59
__57:
%__61 = load i32, i32* %num
%__62 = load double, double* %dec
%__64 = sitofp i32 %__61 to double
%__63 = fadd double %__64, %__62
%__66 = sitofp i32 0 to double
%__65 = fcmp oeq double %__63, %__66
br i1 %__65, label %__68, label %__67
__68:
br label %__69
__67:
%__71 = load double, double* %dec
%__72 = fdiv double %__71, 0.0
%__73 = load i32, i32* %num
%__75 = sitofp i32 %__73 to double
%__74 = fdiv double %__75, 0.0
%__76 = fcmp olt double %__72, %__74
br label %__69
__69:
%__70 = phi i1 [ 1, %__68], [%__76, %__67]
br label %__59
__59:
%__60 = phi i1 [ 0, %__58], [%__70, %__57]
store i1 %__60, i1* %cond
%__80 = load i1, i1* %cond
%__82 = xor i1 %__80, 1
br i1 %__82, label %__77, label %__78
__77:
ret i32 0
br label %__79
__78:
br label %__79
__79:
%__83 = load i1, i1* %cond
br i1 %__83, label %__84, label %__85
__84:
call i32 (i8*, ...) @printf(i8* bitcast ([5 x i8]* @true to i8*))
br label %__86
__85:
call i32 (i8*, ...) @printf(i8* bitcast ([6 x i8]* @false to i8*))
br label %__86
__86:
%__87 = sitofp i32 1 to double
store double %__87, double* %temp
store double 2.0, double* %temp
%__88 = fadd double %__87, 2.0
%__89 = fsub double %__88, 2.0
%__90 = load double, double* %dec
%__92 = fneg double %__90
%__94 = fneg double %__92
%__95 = load double, double* %dec
%__97 = fneg double %__95
%__99 = fneg double %__97
%__101 = fneg double %__99
%__103 = fneg double %__101
%__104 = fcmp oeq double %__94, %__103
%__106 = zext i1 %__104 to i32
%__107 = mul i32 10, %__106
%__109 = sitofp i32 %__107 to double
%__108 = fadd double %__89, %__109
%__110 = load i32, i32* %num
%__111 = load i32, i32* %num
%__112 = icmp slt i32 %__110, %__111
%__114 = xor i1 %__112, 1
%__116 = zext i1 %__114 to i32
%__117 = mul i32 100, %__116
%__119 = sitofp i32 %__117 to double
%__118 = fadd double %__108, %__119
%__120 = load i32, i32* %num
%__121 = load i32, i32* %num
%__122 = icmp sgt i32 %__120, %__121
%__123 = icmp eq i1 %__122, 1
%__125 = zext i1 %__123 to i32
%__126 = mul i32 1000, %__125
%__128 = sitofp i32 %__126 to double
%__127 = fadd double %__118, %__128
%__129 = load i32, i32* %num
%__130 = load i32, i32* %num
%__132 = xor i32 %__130, -2147483648
%__133 = or i32 %__129, %__132
%__134 = load double, double* %dec
%__136 = sitofp i32 1 to double
%__135 = fsub double %__134, %__136
%__138 = sitofp i32 %__133 to double
%__137 = fcmp olt double %__138, %__135
%__140 = zext i1 %__137 to i32
%__141 = mul i32 10000, %__140
%__143 = sitofp i32 %__141 to double
%__142 = fadd double %__127, %__143
%__144 = load i32, i32* %num
%__145 = add i32 %__144, 2
%__146 = load i32, i32* %num
%__147 = add i32 %__146, 2
%__148 = and i32 %__145, %__147
%__149 = icmp eq i32 %__148, 2
%__151 = zext i1 %__149 to i32
%__152 = mul i32 100000, %__151
%__154 = sitofp i32 %__152 to double
%__153 = fadd double %__142, %__154
%__155 = load i32, i32* %num
%__157 = xor i32 %__155, -2147483648
%__159 = sub i32 0, %__157
%__160 = load i32, i32* %num
%__162 = xor i32 %__160, -2147483648
%__163 = sdiv i32 %__159, %__162
%__165 = sub i32 0, 1
%__166 = icmp eq i32 %__163, %__165
%__168 = zext i1 %__166 to i32
%__169 = mul i32 1000000, %__168
%__171 = sitofp i32 %__169 to double
%__170 = fadd double %__153, %__171
%__172 = load i32, i32* %num
%__173 = load i32, i32* %num
%__175 = xor i32 %__173, -2147483648
%__177 = sub i32 0, %__175
%__178 = load i32, i32* %num
%__180 = xor i32 %__178, -2147483648
%__182 = sub i32 0, %__180
%__183 = or i32 %__177, %__182
%__184 = sdiv i32 %__172, %__183
%__187 = mul i32 10000000, %__184
%__189 = sitofp i32 %__187 to double
%__188 = fadd double %__170, %__189
%__190 = load i32, i32* %num
%__192 = xor i32 %__190, -2147483648
%__194 = sub i32 0, %__192
%__195 = load i32, i32* %num
%__197 = sub i32 0, %__195
%__199 = xor i32 %__197, -2147483648
%__200 = mul i32 %__194, %__199
%__201 = icmp sge i32 %__200, 0
%__203 = zext i1 %__201 to i32
%__204 = mul i32 100000000, %__203
%__206 = sitofp i32 %__204 to double
%__205 = fadd double %__188, %__206
store double %__205, double* %temp
%__207 = load double, double* %temp
%__208 = fcmp olt double %__207, 1100111.00000001
br i1 %__208, label %__209, label %__210
__210:
br label %__211
__209:
%__213 = load double, double* %temp
%__214 = fcmp ogt double %__213, 1100110.99999999
br label %__211
__211:
%__212 = phi i1 [ 0, %__210], [%__214, %__209]
br i1 %__212, label %__216, label %__215
__216:
br label %__217
__215:
%__219 = load i1, i1* %cond
%__221 = xor i1 %__219, 1
br label %__217
__217:
%__218 = phi i1 [ 1, %__216], [%__221, %__215]
store i1 %__218, i1* %cond
%__225 = load i1, i1* %cond
br i1 %__225, label %__222, label %__223
__222:
call i32 (i8*, ...) @printf(i8* bitcast ([12 x i8]* @__1 to i8*))
store i32 2147483647, i32* %num
%__226 = load i32, i32* %num
%__227 = sitofp i32 %__226 to double
store double %__227, double* %dec
%__228 = load i32, i32* %num
%__229 = load double, double* %dec
%__231 = fptosi double %__229 to i32
%__232 = icmp eq i32 %__228, %__231
br i1 %__232, label %__233, label %__234
__234:
br label %__235
__233:
%__237 = load i32, i32* %num
%__238 = add i32 %__237, 1
%__239 = icmp slt i32 %__238, 0
br label %__235
__235:
%__236 = phi i1 [ 0, %__234], [%__239, %__233]
br i1 %__236, label %__240, label %__241
__241:
br label %__242
__240:
%__244 = load double, double* %dec
%__246 = sitofp i32 1 to double
%__245 = fadd double %__244, %__246
%__248 = sitofp i32 0 to double
%__247 = fcmp ogt double %__245, %__248
br label %__242
__242:
%__243 = phi i1 [ 0, %__241], [%__247, %__240]
store i1 %__243, i1* %cond
%__252 = load i1, i1* %cond
br i1 %__252, label %__249, label %__250
__249:
call i32 (i8*, ...) @printf(i8* bitcast ([17 x i8]* @__2 to i8*))
%__254 = xor i32 2147483647, -2147483648
%__256 = xor i32 %__254, -2147483648
%__257 = and i32 %__256, 0
%__258 = mul i32 2147483647, 0
%__259 = icmp eq i32 %__257, %__258
%__261 = xor i1 %__259, 1
%__263 = xor i1 %__261, 1
%__264 = icmp ne i1 %__263, 0
store i1 %__264, i1* %cond
%__268 = load i1, i1* %cond
br i1 %__268, label %__265, label %__266
__265:
call i32 (i8*, ...) @printf(i8* bitcast ([20 x i8]* @__3 to i8*))
%__269 = load i1, i1* %cond
%__271 = xor i1 %__269, 1
%__273 = zext i1 %__271 to i32
%__275 = xor i32 %__273, -2147483648
%__277 = sub i32 0, %__275
%__278 = icmp eq i32 1, %__277
store i1 %__278, i1* %cond
%__282 = load i1, i1* %cond
br i1 %__282, label %__279, label %__280
__279:
call i32 (i8*, ...) @printf(i8* bitcast ([17 x i8]* @__4 to i8*))
%__283 = load i32, i32* %num
%__284 = sdiv i32 %__283, 2
%__285 = add i32 %__284, 2
%__288 = and i32 1, %__285
%__289 = and i32 %__288, 1
store i32 %__289, i32* %num
store i1 1, i1* %cond
%__293 = zext i1 1 to i32
%__294 = icmp eq i32 1, %__293
store i1 0, i1* %cond
%__295 = icmp ne i1 %__294, 0
store i1 1, i1* %cond
%__296 = icmp eq i1 %__295, 1
store i1 %__296, i1* %cond
br label %__297
__297:
%__300 = load i1, i1* %cond
br i1 %__300, label %__298, label %__299
__298:
call i32 (i8*, ...) @printf(i8* bitcast ([46 x i8]* @__5 to i8*))
%__301 = load i32, i32* %num
%__303 = xor i32 %__301, -2147483648
%__305 = sub i32 0, %__303
%__306 = icmp eq i32 %__305, 2
%__308 = xor i1 %__306, 1
store i1 %__308, i1* %cond
br label %__297
__299:
br label %__281
__280:
br label %__281
__281:
br label %__267
__266:
br label %__267
__267:
br label %__251
__250:
br label %__251
__251:
br label %__224
__223:
br label %__224
__224:
call i32 (i8*, ...) @printf(i8* bitcast ([20 x i8]* @__6 to i8*))

ret i32 0
}
