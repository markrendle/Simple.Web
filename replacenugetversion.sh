find . -name "*.nuspec" -maxdepth 1 -exec sed -i "s/$1/$2/" {} \;
