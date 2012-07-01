default:
	mdtool build -p:checkov -c:"Release|x86"
bundle: default
	mkbundle -o checkov --static --deps -z ./bin/Release/checkov.exe
	
