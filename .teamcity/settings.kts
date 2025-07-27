import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.buildFeatures.*
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.*
import jetbrains.buildServer.configs.kotlin.v2019_2.vcs.*


// TeamCity Kotlin DSL version
version = "2020.1"

// Build settings
open class Settings {
    companion object {
        const val configuration = "Release"

        const val gitRepo = "https://github.com/ace90210/senior-engineer-home-exercise.git"
        const val gitBranch = "refs/heads/master"
        const val versionPrefix = "1.0.0"
        const val versionSuffix = "beta%build.number%"
    }
}

project {
    vcsRoot(GitHubRepo)
    buildType(BuildWebLinux64)
}

object GitHubRepo : GitVcsRoot({
    name = "senior-engineer-home-exercise"
    url = Settings.gitRepo
    branch = Settings.gitBranch
})

open class BuildBase(
    requiresSdk: Boolean
) : BuildType() {
    constructor(requiresSdk: Boolean, init: BuildBase.() -> Unit) : this(requiresSdk) {
        init()
    }

    init {
        vcs { root(GitHubRepo) }

        features {
            swabra {
                forceCleanCheckout = true
            }
        }

        if (requiresSdk) {
            requirements {
                exists("DotNetCoreSDK8.0_Path")
            }
        }
    }
}

open class BuildWebBase(
    runtimeId: String
) : BuildBase(true) {
    init {
        name = "Build console and web for $runtimeId"

        params {
            param("system.InvariantGlobalization", "true")
        }

       steps {
       

            step {
                name = "Print Script Version"
                type = "simpleRunner"
                param("script.content", """
                    echo "🔥 Version check: v0.1.6"
                """.trimIndent())
            } 
            step {
                name = "Move Solution to Root"
                type = "simpleRunner"
                param("script.content", """
                    echo "🛠 Moving src/* to working directory root..."
                    cp -r src/* .
                """.trimIndent())
            }

            step {
                type = "dotnet"
                param("command", "restore")
                param("projects", "People.sln")
            }

            step {
                type = "dotnet"
                param("command", "build")
                param("projects", "People.sln")
                param("args", "--configuration ${Settings.configuration}")
            }

            step {
                type = "dotnet"
                param("command", "test")
                param("projects", "People.sln")
                param("args", "--configuration ${Settings.configuration} --logger trx --results-directory test-results")
            }

            step {
                name = "Docker Build and Push"
                type = "simpleRunner"
                param("script.content", """
                    echo "🐳 Building Docker image..."
                    docker build -t localhost:5000/people-app:latest .

                    echo "📤 Pushing image to local registry..."
                    docker push localhost:5000/people-app:latest
                """.trimIndent())
            }
        }

        artifactRules = "test-results => test-results"

        features {
            feature {
                type = "xml-report-plugin"
                param("xmlReportParsing.reportType", "vstest")
                param("xmlReportParsing.reportDirs", "test-results/*.trx")
            }
        }
    }
}

// Actual build configuration
object BuildWebLinux64 : BuildWebBase("linux-x64")
