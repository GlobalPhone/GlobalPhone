require 'visual_studio_files.rb'
require 'albacore'

require 'rbconfig'
#http://stackoverflow.com/questions/11784109/detecting-operating-systems-in-ruby
def os
  @os ||= (
    host_os = RbConfig::CONFIG['host_os']
    case host_os
    when /mswin|msys|mingw|cygwin|bccwin|wince|emc/
      :windows
    when /darwin|mac os/
      :macosx
    when /linux/
      :linux
    when /solaris|bsd/
      :unix
    else
      raise Error::WebDriverError, "unknown os: #{host_os.inspect}"
    end
  )
end

def nuget_exec(parameters)

  command = File.join(File.dirname(__FILE__), "src",".nuget","NuGet.exe")
  if os == :windows
    sh "#{command} #{parameters}"
  else
    sh "mono --runtime=v4.0.30319 #{command} #{parameters} "
  end
end

def nunit_cmd()
  cmds = Dir.glob(File.join(File.dirname(__FILE__),"src","packages","NUnit.Runners.*","tools","nunit-console.exe"))
  if cmds.any?
    if os != :windows
      command = "mono --runtime=v4.0.30319 #{cmds.first}"
    else
      command = cmds.first
    end
  else
    raise "Could not find nunit runner!"
  end
  return command
  
end

def nunit_exec(dir, tlib)
    command = nunit_cmd()
    assemblies= "#{tlib}.dll"
    cd dir do
      sh "#{command} #{assemblies}" do  |ok, res|
        if !ok
          abort 'Nunit failed!'
        end
      end
    end

end

def with_mono_properties msb
  solution_dir = File.join(File.dirname(__FILE__),'src')
  nuget_tools_path = File.join(solution_dir, '.nuget')
  msb.prop :SolutionDir, solution_dir
  msb.prop :NuGetToolsPath, nuget_tools_path
  msb.prop :NuGetExePath, File.join(nuget_tools_path, 'NuGet.exe')
  msb.prop :PackagesDir, File.join(solution_dir, 'packages')
end

desc "build"
build :build do |msb|
  msb.prop :configuration, :Debug
  msb.prop :platform, "Mixed Platforms"
  if os != :windows
    with_mono_properties msb
  end
  msb.target = :Rebuild
  msb.be_quiet
  msb.nologo
  msb.sln =File.join(dir,"src", "GlobalPhone.sln")
end

desc "test using nunit console"
task :test => :build do |t|
  assemblies = "GlobalPhone.Tests.dll"
  dir = File.join('.',"src","GlobalPhone.Tests","bin","Debug")
  nunit_exec(dir,"GlobalPhone.Tests")
end

task :main_copy_to_nuspec => [:build] do
  dir = File.dirname(__FILE__)
  output_directory_lib = File.join(dir,"nuget/lib/40/")
  mkdir_p output_directory_lib
  cp Dir.glob("./src/GlobalPhone/bin/Debug/GlobalPhone.dll"), output_directory_lib
end

task :tool_copy_to_nuspec => [:build] do
  dir = File.dirname(__FILE__)
  output_directory_tools = File.join(dir,"nuget_tool/tools/")
  mkdir_p output_directory_tools
  cp Dir.glob("./src/GlobalPhoneDbgen/bin/Debug/*.dll"), output_directory_tools
  cp Dir.glob("./src/GlobalPhoneDbgen/bin/Debug/GlobalPhoneDbgen.exe"), output_directory_tools
end

task :main_nugetpack => [:main_copy_to_nuspec] do |nuget|
  dir = File.dirname(__FILE__)
  cd File.join(dir,"nuget") do
    nuget_exec "pack GlobalPhone.nuspec"
  end
end

task :tool_nugetpack => [:tool_copy_to_nuspec] do |nuget|
  dir = File.dirname(__FILE__)
  cd File.join(dir,"nuget_tool") do
    nuget_exec "pack GlobalPhoneDbgen.nuspec"
  end
end

desc "create the nuget package"
task :nugetpack => [:main_nugetpack, :tool_nugetpack]

desc "Install missing NuGet packages."
task :install_packages do
  package_paths = FileList["src/**/packages.config"]+["src/.nuget/packages.config"]

  package_paths.each.each do |filepath|
    begin
      nuget_exec("i #{filepath} -o ./src/packages -source http://www.nuget.org/api/v2/")
    rescue
      puts "Failed to install missing packages ..."      
    end
  end
end

desc "regenerate links"
task :regen_links => [:regen_links_dbgen]

desc "regenerate links in dbgen"
task :regen_links_dbgen do
  dir = File.dirname(__FILE__)
  global_phone = VisualStudioFiles::CsProj.new(File.open(File.join(dir,'src','GlobalPhone','GlobalPhone.csproj'), "r").read)
  global_phone_files = global_phone.files.select do |file|
    file.type=='Compile' && !file.file.end_with?('AssemblyInfo.cs')
  end
    
  global_phone_dbgen = VisualStudioFiles::CsProj.new(File.open(File.join(dir,'src','GlobalPhoneDbgen','GlobalPhoneDbgen.csproj'), "r").read)
  global_phone_dbgen.clear_links
  global_phone_files.each do |file|
    hash = file.to_hash
    hash[:file] = "..\\GlobalPhone\\#{file.file}"
    hash[:link] = "GlobalPhone\\#{file.file}"
    global_phone_dbgen.add(hash)
  end
  File.open(File.join(dir,'src','GlobalPhoneDbgen','GlobalPhoneDbgen.csproj'), "w") do |f|
    global_phone_dbgen.write f
  end
end

