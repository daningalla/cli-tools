assembly=${PWD##*/}
dotnet tool uninstall -g $assembly
dotnet pack -c release -o ./package
dotnet tool install -g --add-source ./package $assembly