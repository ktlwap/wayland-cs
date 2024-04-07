# Wayland Implementation in C#

This project is a C# implementation of the Wayland protocol, a protocol that specifies the communication between a display server and its clients. The project is divided into two main parts: the Wayland Server and the Wayland Client.
File descriptors are not supported yet due to missing support in System.Net.Sockets, it'll be added at a later point.

## Wayland Server

The server is responsible for managing the display, handling client requests, and providing the necessary resources for the clients. The server's main class is `Display`, which is responsible for managing the Unix socket connections.

## Wayland Client

The client is responsible for communicating with the server, sending requests, and handling the responses. The client's main class is `Connection`, which is responsible for managing the socket connection with the server and handling the event queue.

## Simple Client Example

The `Simple.Client` project is an example of how to use the Wayland Client to connect to a Wayland Server, bind to the display and registry, and handle global events and callbacks.

## Building the Project

The project is built using .NET 8.0. To build the project, navigate to the project's root directory and run the following command:

```bash
dotnet build
```

## Running the Simple Client Example

To run the Simple Client example, navigate to the `examples/Simple.Client` directory and run the following command:

```bash
dotnet run
```

## Dependencies

The project depends on the `Wayland.Protocol.Client` project, which is included as a project reference in the `Wayland.Client` project file.

## Environment Variables

The project uses the `XDG_RUNTIME_DIR` and `WAYLAND_DISPLAY` environment variables to determine the Unix socket path for the Wayland server.

## Contributing

Contributions are welcome. Please submit a pull request with your changes.

## License

This project is licensed under the MIT License.
