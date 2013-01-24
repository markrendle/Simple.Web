include FileTest

BASE_PATH = File.expand_path(File.dirname(__FILE__))
COMMON_ASSEMBLY_INFO = "#{BASE_PATH}/../src/CommonAssemblyInfo.cs"

File.open(COMMON_ASSEMBLY_INFO, "w") {} unless FileTest.exists?(COMMON_ASSEMBLY_INFO)